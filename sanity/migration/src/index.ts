import {writeFileSync} from 'node:fs'
import {join} from 'node:path'
import {isDryRun, migrationDir, readSanityEnv, resolveExportPath} from './config.js'
import {entriesOfType, loadExport} from './contentful.js'
import {uploadAndEmbed} from './assets.js'
import {makeClient, uploadDocs} from './sanity.js'
import {
  ImageManifest,
  SanityDoc,
  transformBlogPost,
  transformCategory,
  transformImageAsset,
  transformPage,
} from './transformers.js'

async function main(): Promise<void> {
  const exportPath = resolveExportPath()
  console.log(`Reading Contentful export:\n  ${exportPath}`)
  const load = loadExport(exportPath)

  const manifest = new ImageManifest(load)
  const warnings: string[] = []
  const warn = (msg: string) => warnings.push(msg)

  // Build docs. Order matters for upload: referenced docs first.
  const categories = entriesOfType(load, 'category').map(transformCategory)
  const imageAssets = entriesOfType(load, 'image').map((e) =>
    transformImageAsset(e, manifest),
  )
  const pages = entriesOfType(load, 'page').map((e) =>
    transformPage(e, load, manifest, warn),
  )
  const blogPosts = entriesOfType(load, 'blogpost').map((e) =>
    transformBlogPost(e, load, manifest, warn),
  )

  const ordered: SanityDoc[] = [...categories, ...imageAssets, ...pages, ...blogPosts]

  console.log('\nTransformed documents:')
  console.log(`  category    ${categories.length}`)
  console.log(`  imageAsset  ${imageAssets.length}`)
  console.log(`  page        ${pages.length}`)
  console.log(`  blogPost    ${blogPosts.length}`)
  console.log(`  total       ${ordered.length}`)

  // Always write the image manifest for the later binary-upload step.
  const manifestPath = join(migrationDir, 'image-manifest.json')
  writeFileSync(manifestPath, JSON.stringify(manifest.toArray(), null, 2))
  console.log(`\nImage manifest: ${manifest.size} assets -> ${manifestPath}`)

  if (warnings.length > 0) {
    console.log(`\nWarnings (${warnings.length}):`)
    for (const w of warnings) console.log(`  - ${w}`)
  }

  if (isDryRun) {
    const ndjsonPath = join(migrationDir, 'output.ndjson')
    writeFileSync(ndjsonPath, ordered.map((d) => JSON.stringify(d)).join('\n') + '\n')
    console.log(
      `\nDry run — wrote ${ordered.length} docs to ${ndjsonPath} (image placeholders only). ` +
        `No data sent to Sanity.`,
    )
    return
  }

  const env = readSanityEnv()
  const client = makeClient(env)

  // Upload image binaries and embed their references directly into the docs
  // before writing, so a single `migrate` run produces fully-imaged documents.
  console.log('\nUploading image binaries and embedding references ...')
  const {uploaded, embedded, failures} = await uploadAndEmbed(
    client,
    ordered,
    manifest.toArray(),
    (m) => console.log(`  ${m}`),
  )
  console.log(`Images: uploaded ${uploaded} assets, embedded ${embedded} references.`)

  console.log(`\nUploading to Sanity project ${env.projectId} / dataset ${env.dataset} ...`)
  await uploadDocs(client, ordered)
  console.log(`Done. Upserted ${ordered.length} documents.`)
  if (failures.length > 0) {
    console.log(`\nImage failures (${failures.length}) — these docs kept empty image fields:`)
    for (const f of failures) console.log(`  - ${f}`)
    process.exitCode = 1
  }
}

main().catch((err) => {
  console.error('\nMigration failed:')
  console.error(err)
  process.exitCode = 1
})
