# Contentful → Sanity migration

A one-off TypeScript app that reads the Contentful export JSON and writes the equivalent
documents into Sanity. Part of the migration off Contentful for basementmedia.no.

## What it migrates

| Contentful | Sanity | Notes |
|---|---|---|
| `category` | `category` | title + slug |
| `image` (content type) | `imageAsset` | title, `altText`→alt, caption, `photographer`→credit |
| `page` | `page` | title, slug, body, includeInSearchAndNavigation |
| `blogpost` | `blogPost` | + categories (refs), `image` (ref to imageAsset), publishedAt, intro |
| `codeBlock` (embedded) | `{_type:'code'}` block | language + code, inline in body |

Contentful IDs are reused as Sanity `_id`s, so references resolve directly and re-runs are
idempotent (`createOrReplace`).

### Rich text → Portable Text
Headings h1–h6, blockquote, bullet/number lists, and the strong/em/underline/code marks map
directly. Hyperlinks become `link` annotations. Embedded `codeBlock` entries become `code` blocks.
`hr` nodes are dropped (no schema equivalent). Internal `entry-hyperlink`s resolve to `/<slug>`.

### Images
`pnpm migrate` uploads image binaries and embeds their references **inline**, so a single run
produces fully-imaged documents. For every referenced Contentful asset it:
- uploads the binary to Sanity (Sanity dedupes identical bytes, so re-runs are safe), then
- sets the asset reference directly on the in-memory document before it is written:
  - `imageAsset.image` and `mainImage` by field,
  - body images by their block `_key`,
  - **body links** whose href points at a Contentful asset CDN URL (`*.ctfassets.net`, which dies
    after the migration) are repointed to the Sanity asset URL. Ordinary external links and
    internal `/slug` links are left untouched (the schema's `link.href` allows relative URLs).

Every referenced asset is also recorded in `image-manifest.json` (url, filename, dimensions, and
which docs/fields use it, including block + markDef `_key`s).

`pnpm upload-images` re-runs **only** the image step against documents already in Sanity: it reads
the manifest, fetches those live documents, embeds the asset references, and writes them back. Use
it if the image step failed partway, or to re-apply images without re-importing content. Because it
fetches the live docs, block `_key`s always match — there is no coupling to a particular `migrate`
run.

> A real `migrate` run needs network access and downloads ~35 images, so it is slower than a
> content-only import; that's expected for this one-off.

## Prerequisites
The Studio schema must include the `code` type (added via `@sanity/code-input`). Run
`pnpm install` in `../studio` if you haven't since that change.

## Usage

```powershell
pnpm install

# 1. Dry run — writes output.ndjson (image placeholders) + image-manifest.json,
#    sends nothing to Sanity.
pnpm migrate:dry

# Copy .env.example to .env and fill in SANITY_PROJECT_ID, SANITY_DATASET,
# and a write-enabled SANITY_WRITE_TOKEN before the real run.

# 2. Migrate everything: content, structure, AND image binaries in one run.
pnpm migrate

# Optional: re-run ONLY the image step against docs already in Sanity
# (e.g. if image uploads failed partway).
pnpm upload-images:dry   # preview the plan
pnpm upload-images
```

The Contentful export path is auto-detected (newest `contentful-export-*.json` under
`../../Contentful/exports`); override with `CONTENTFUL_EXPORT` in `.env`.
