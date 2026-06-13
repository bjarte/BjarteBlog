# basementmedia.no — Sanity Studio

A minimal [Sanity](https://www.sanity.io/) Studio for basementmedia.no. This is the authoring UI only — the front-end (.NET app in
`../Blog`) will be pointed at Sanity in a later step.

## Document types

- **Blog Post** — copied field-for-field from the Contentful `blogpost` model, with
  the rich-text `body` as Portable Text.
- **Category** — title + slug, used to categorize blog posts.
- **Image** (`imageAsset`) — a reusable image document with alt text, caption and credit.
- **Page** — a generic content page.

## One-time setup

The Studio needs a Sanity project to connect to. After installing dependencies:

Find the Sanity project id at sanity.io. Copy `.env-example` to `.env` and add a valid project id.

```bash
# Install pnpm and Sanity globally with Mise
mise use --global node pnpm npm:sanity
pnpm install
```

## Commands

```bash
pnpm dev      # run the Studio locally at http://localhost:3333
pnpm build    # build the Studio for production
pnpm deploy   # deploy to <project>.sanity.studio
```

## Project structure

```text
sanity.config.ts        Studio config (plugins + schema registration)
sanity.cli.ts           CLI config (projectId + dataset)
schemaTypes/
  index.ts              registers all schema types
  blockContent.ts       shared Portable Text type (used by blogPost + page bodies)
  blogPost.ts
  category.ts
  imageAsset.ts
  page.ts
```
