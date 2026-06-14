import {CfAsset, CfAssetFile, CfEntry, CfLink, CfNode, LoadedExport, loc} from './contentful.js'
import {
  ConvertContext,
  genKey,
  PtMember,
  richTextToPortableText,
} from './portableText.js'

// --- Image manifest ------------------------------------------------------------

export interface ManifestUse {
  docId: string
  field: string
  /** `_key` of the body block (image block, or block holding a link). */
  key?: string
  /** `_key` of the link markDef, when `field` is "body-link-href". */
  markKey?: string
}

export interface ManifestEntry {
  contentfulAssetId: string
  url?: string
  fileName?: string
  contentType?: string
  width?: number
  height?: number
  size?: number
  title?: string
  description?: string
  usedBy: ManifestUse[]
}

function httpsUrl(url: string): string {
  return url.startsWith('//') ? `https:${url}` : url
}

/** Collects every Contentful asset that a migrated document refers to. */
export class ImageManifest {
  private readonly byId = new Map<string, ManifestEntry>()

  constructor(private readonly load: LoadedExport) {}

  record(
    assetId: string,
    docId: string,
    field: string,
    opts?: {key?: string; markKey?: string},
  ): void {
    let entry = this.byId.get(assetId)
    if (!entry) {
      entry = {contentfulAssetId: assetId, usedBy: []}
      const asset: CfAsset | undefined = this.load.assetsById.get(assetId)
      const cfFile = asset ? loc<CfAssetFile>(asset.fields.file) : undefined
      if (cfFile) {
        entry.url = httpsUrl(cfFile.url)
        entry.fileName = cfFile.fileName
        entry.contentType = cfFile.contentType
        entry.size = cfFile.details?.size
        entry.width = cfFile.details?.image?.width
        entry.height = cfFile.details?.image?.height
      }
      if (asset) {
        entry.title = loc<string>(asset.fields.title)
        entry.description = loc<string>(asset.fields.description)
      }
      this.byId.set(assetId, entry)
    }
    const use: ManifestUse = {docId, field}
    if (opts?.key) use.key = opts.key
    if (opts?.markKey) use.markKey = opts.markKey
    entry.usedBy.push(use)
  }

  toArray(): ManifestEntry[] {
    return [...this.byId.values()]
  }

  get size(): number {
    return this.byId.size
  }
}

// --- Helpers -------------------------------------------------------------------

function slugObject(current: string | undefined) {
  return current ? {_type: 'slug' as const, current} : undefined
}

/** Normalize a Contentful Date (may be date-only) to an ISO datetime. */
function toDateTime(value: string | undefined): string | undefined {
  if (!value) return undefined
  return /^\d{4}-\d{2}-\d{2}$/.test(value) ? `${value}T00:00:00Z` : value
}

/** Strip undefined values so we don't write empty keys into Sanity. */
function clean<T extends Record<string, unknown>>(obj: T): T {
  for (const k of Object.keys(obj)) {
    if (obj[k] === undefined) delete obj[k]
  }
  return obj
}

function assetIdOf(link: CfLink | undefined): string | undefined {
  return link?.sys.linkType === 'Asset' ? link.sys.id : undefined
}

// --- Document transforms -------------------------------------------------------

export interface SanityDoc {
  _id: string
  _type: string
  [key: string]: unknown
}

export function transformCategory(entry: CfEntry): SanityDoc {
  return clean({
    _id: entry.sys.id,
    _type: 'category',
    title: loc<string>(entry.fields.title),
    slug: slugObject(loc<string>(entry.fields.slug)),
  })
}

export function transformImageAsset(
  entry: CfEntry,
  manifest: ImageManifest,
): SanityDoc {
  const assetLink = loc<CfLink>(entry.fields.image)
  const assetId = assetIdOf(assetLink)
  if (assetId) manifest.record(assetId, entry.sys.id, 'imageAsset.image')

  const image = clean({
    _type: 'image',
    alt: loc<string>(entry.fields.altText),
    caption: loc<string>(entry.fields.caption),
  })

  return clean({
    _id: entry.sys.id,
    _type: 'imageAsset',
    title: loc<string>(entry.fields.title),
    credit: loc<string>(entry.fields.photographer),
    // No `asset` yet — binary uploaded in the later step.
    image,
  })
}

function buildConvertContext(
  entry: CfEntry,
  load: LoadedExport,
  manifest: ImageManifest,
  warn: (msg: string) => void,
): {ctx: ConvertContext; body: (field: CfNode | undefined) => PtMember[]} {
  const ctx: ConvertContext = {
    load,
    docId: entry.sys.id,
    recordAsset: (assetId, field, opts) =>
      manifest.record(assetId, entry.sys.id, field, opts),
    warn: (msg) => warn(`[${entry.sys.id}] ${msg}`),
  }
  return {ctx, body: (field) => richTextToPortableText(field, ctx)}
}

export function transformBlogPost(
  entry: CfEntry,
  load: LoadedExport,
  manifest: ImageManifest,
  warn: (msg: string) => void,
): SanityDoc {
  const {body} = buildConvertContext(entry, load, manifest, warn)

  const categoryLinks = loc<CfLink[]>(entry.fields.categories) ?? []
  const categories = categoryLinks.map((link) => ({
    _type: 'reference' as const,
    _key: genKey(),
    _ref: link.sys.id,
  }))

  const imageEntryId = loc<CfLink>(entry.fields.image)?.sys.id

  const mainImageId = assetIdOf(loc<CfLink>(entry.fields.mainImage))
  if (mainImageId) manifest.record(mainImageId, entry.sys.id, 'blogPost.mainImage')

  return clean({
    _id: entry.sys.id,
    _type: 'blogPost',
    title: loc<string>(entry.fields.title),
    slug: slugObject(loc<string>(entry.fields.slug)),
    publishedAt: toDateTime(loc<string>(entry.fields.publishedAt)),
    intro: loc<string>(entry.fields.intro),
    includeInSearchAndNavigation:
      loc<boolean>(entry.fields.includeInSearchAndNavigation) ?? true,
    categories: categories.length > 0 ? categories : undefined,
    image: imageEntryId
      ? {_type: 'reference', _ref: imageEntryId}
      : undefined,
    // mainImage deferred (binary uploaded later; recorded in manifest).
    body: body(loc<CfNode>(entry.fields.body)),
  })
}

export function transformPage(
  entry: CfEntry,
  load: LoadedExport,
  manifest: ImageManifest,
  warn: (msg: string) => void,
): SanityDoc {
  const {body} = buildConvertContext(entry, load, manifest, warn)

  const mainImageId = assetIdOf(loc<CfLink>(entry.fields.mainImage))
  if (mainImageId) manifest.record(mainImageId, entry.sys.id, 'page.mainImage')

  return clean({
    _id: entry.sys.id,
    _type: 'page',
    title: loc<string>(entry.fields.title),
    slug: slugObject(loc<string>(entry.fields.slug)),
    includeInSearchAndNavigation:
      loc<boolean>(entry.fields.includeInSearchAndNavigation) ?? true,
    // mainImage deferred (binary uploaded later; recorded in manifest).
    body: body(loc<CfNode>(entry.fields.body)),
  })
}
