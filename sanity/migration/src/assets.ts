import {SanityClient} from '@sanity/client'
import {ManifestEntry, ManifestUse, SanityDoc} from './transformers.js'

/** Manifest fields the embed step knows how to wire to an uploaded asset. */
export const PATCHABLE = new Set([
  'imageAsset.image',
  'blogPost.mainImage',
  'page.mainImage',
  'body-image',
  'body-link-href',
])

const ref = (assetId: string) => ({_type: 'reference' as const, _ref: assetId})

export interface UploadedAsset {
  id: string
  url: string
}

export async function uploadAsset(
  client: SanityClient,
  entry: ManifestEntry,
  attempts = 3,
): Promise<UploadedAsset> {
  if (!entry.url) throw new Error(`Asset ${entry.contentfulAssetId} has no url`)

  // Only include metadata that is actually present — Sanity rejects empty
  // `title`/`description` strings with a 400 "not allowed to be empty".
  const options: Record<string, string> = {}
  if (entry.fileName) options.filename = entry.fileName
  if (entry.contentType) options.contentType = entry.contentType
  if (entry.title) options.title = entry.title
  if (entry.description) options.description = entry.description

  let lastErr: unknown
  for (let attempt = 1; attempt <= attempts; attempt++) {
    try {
      const res = await fetch(entry.url)
      if (!res.ok) throw new Error(`Download failed (${res.status}) for ${entry.url}`)
      const buffer = Buffer.from(await res.arrayBuffer())
      // Sanity dedupes identical bytes by content hash, so re-runs are safe.
      const asset = await client.assets.upload('image', buffer, options)
      return {id: asset._id, url: asset.url}
    } catch (err) {
      lastErr = err
      // Retry only transient network errors, not validation (400) failures.
      const transient = /ECONNRESET|ETIMEDOUT|EAI_AGAIN|socket hang up|fetch failed/i.test(
        (err as Error).message,
      )
      if (!transient || attempt === attempts) throw err
    }
  }
  throw lastErr
}

function bodyMember(doc: SanityDoc, key: string | undefined): Record<string, unknown> | undefined {
  if (!key) return undefined
  const body = doc.body as Record<string, unknown>[] | undefined
  return body?.find((m) => m._key === key)
}

/**
 * Mutate `doc` in place so the given usage points at the uploaded asset.
 * Returns whether the reference was actually applied.
 */
export function embedInDoc(
  doc: SanityDoc | undefined,
  use: ManifestUse,
  asset: UploadedAsset,
  alt?: string,
): boolean {
  if (!doc) return false
  switch (use.field) {
    case 'imageAsset.image': {
      const image = (doc.image as Record<string, unknown>) ?? (doc.image = {_type: 'image'})
      image.asset = ref(asset.id)
      return true
    }
    case 'blogPost.mainImage':
    case 'page.mainImage': {
      const mainImage: Record<string, unknown> = {_type: 'image', asset: ref(asset.id)}
      if (alt) mainImage.alt = alt
      doc.mainImage = mainImage
      return true
    }
    case 'body-image': {
      const member = bodyMember(doc, use.key)
      if (!member) return false
      member.asset = ref(asset.id)
      return true
    }
    case 'body-link-href': {
      const member = bodyMember(doc, use.key)
      const markDefs = member?.markDefs as Record<string, unknown>[] | undefined
      const markDef = markDefs?.find((m) => m._key === use.markKey)
      if (!markDef) return false
      // Repoint the link from the dead Contentful URL to the Sanity asset URL.
      markDef.href = asset.url
      return true
    }
    default:
      return false
  }
}

export interface EmbedResult {
  uploaded: number
  embedded: number
  failures: string[]
}

/**
 * Upload every referenced binary once and embed its reference into the in-memory
 * documents. Shared by `migrate` (fresh docs) and `upload-images` (live docs).
 */
export async function uploadAndEmbed(
  client: SanityClient,
  docs: SanityDoc[],
  manifest: ManifestEntry[],
  log: (msg: string) => void,
): Promise<EmbedResult> {
  const docsById = new Map(docs.map((d) => [d._id, d]))
  const toUpload = manifest.filter((e) => e.usedBy.some((u) => PATCHABLE.has(u.field)))

  let uploaded = 0
  let embedded = 0
  const failures: string[] = []

  for (const entry of toUpload) {
    try {
      const asset = await uploadAsset(client, entry)
      uploaded++
      const alt = entry.description ?? entry.title
      let applied = 0
      for (const use of entry.usedBy) {
        if (!PATCHABLE.has(use.field)) continue
        if (embedInDoc(docsById.get(use.docId), use, asset, alt)) applied++
      }
      embedded += applied
      log(`✓ ${entry.fileName ?? entry.contentfulAssetId} (${applied} ref${applied === 1 ? '' : 's'})`)
    } catch (err) {
      const msg = `${entry.fileName ?? entry.contentfulAssetId}: ${(err as Error).message}`
      failures.push(msg)
      log(`✗ ${msg}`)
    }
  }

  return {uploaded, embedded, failures}
}
