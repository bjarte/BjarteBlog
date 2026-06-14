import {existsSync, readFileSync} from 'node:fs'
import {join} from 'node:path'
import {PATCHABLE, uploadAndEmbed} from './assets.js'
import {isDryRun, migrationDir, readSanityEnv} from './config.js'
import {makeClient, uploadDocs} from './sanity.js'
import {ManifestEntry, SanityDoc} from './transformers.js'

/**
 * Re-applies images to documents that already exist in Sanity, driven by
 * image-manifest.json. `migrate` already does this inline; use this to (re)run
 * the image step on its own. It fetches the live documents and embeds the asset
 * references into them, so block `_key`s always match (no coupling to a run).
 */
function loadManifest(): ManifestEntry[] {
  const path = join(migrationDir, 'image-manifest.json')
  if (!existsSync(path)) {
    throw new Error(
      `No image-manifest.json at ${path}. Run \`pnpm migrate\` first (it writes the manifest).`,
    )
  }
  return JSON.parse(readFileSync(path, 'utf8')) as ManifestEntry[]
}

async function main(): Promise<void> {
  const manifest = loadManifest()

  const toUpload = manifest.filter((e) => e.usedBy.some((u) => PATCHABLE.has(u.field)))
  const skippedLinkOnly = manifest.length - toUpload.length
  const patchCount = toUpload.reduce(
    (n, e) => n + e.usedBy.filter((u) => PATCHABLE.has(u.field)).length,
    0,
  )

  console.log(`Image manifest: ${manifest.length} assets`)
  console.log(`  to upload (referenced by an image field or asset link): ${toUpload.length}`)
  if (skippedLinkOnly > 0) console.log(`  skipped (no patchable usage): ${skippedLinkOnly}`)
  console.log(`  document references to apply: ${patchCount}`)

  if (isDryRun) {
    console.log('\nDry run — no binaries downloaded, nothing uploaded or patched.')
    for (const e of toUpload) {
      const targets = e.usedBy
        .filter((u) => PATCHABLE.has(u.field))
        .map((u) => `${u.docId}:${u.field}`)
        .join(', ')
      console.log(`  ${e.fileName ?? e.contentfulAssetId} -> ${targets}`)
    }
    return
  }

  const env = readSanityEnv()
  const client = makeClient(env)

  // Fetch the live documents the manifest refers to, embed asset refs, write back.
  const docIds = [
    ...new Set(
      toUpload.flatMap((e) =>
        e.usedBy.filter((u) => PATCHABLE.has(u.field)).map((u) => u.docId),
      ),
    ),
  ]
  console.log(`\nFetching ${docIds.length} documents from Sanity ...`)
  const fetched = (await client.getDocuments(docIds)).filter(
    (d): d is NonNullable<typeof d> => d !== null,
  ) as unknown as SanityDoc[]
  const missing = docIds.length - fetched.length
  if (missing > 0) {
    console.log(`  ${missing} document(s) not found — run \`pnpm migrate\` first.`)
  }

  console.log(`\nUploading images and embedding references ...`)
  const {uploaded, embedded, failures} = await uploadAndEmbed(
    client,
    fetched,
    manifest,
    (m) => console.log(`  ${m}`),
  )

  await uploadDocs(client, fetched)
  console.log(`\nDone. Uploaded ${uploaded} assets, embedded ${embedded} references into ${fetched.length} docs.`)
  if (failures.length > 0) {
    console.log(`\nFailures (${failures.length}):`)
    for (const f of failures) console.log(`  - ${f}`)
    process.exitCode = 1
  }
}

main().catch((err) => {
  console.error('\nImage upload failed:')
  console.error(err)
  process.exitCode = 1
})
