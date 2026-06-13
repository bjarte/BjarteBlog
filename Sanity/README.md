# BjarteBlog — Sanity Studio

A minimal [Sanity](https://www.sanity.io/) Studio for the BjarteBlog content
(basementmedia.no). This is the authoring UI only — the front-end (.NET app in
`../Blog`) will be pointed at Sanity in a later step.

## Document types

- **Blog Post** — copied field-for-field from the Contentful `blogpost` model, with
  the rich-text `body` as Portable Text.
- **Category** — title + slug, used to categorize blog posts.
- **Image** (`imageAsset`) — a reusable image document with alt text, caption and credit.
- **Page** — a generic content page.

## One-time setup

The Studio needs a Sanity project to connect to. After installing dependencies:

```bash
pnpm install

# Log in and create (or link) a project + dataset:
npx sanity login
npx sanity init       # choose "Create new project" (or reuse an existing one),
                      # dataset name "production"
```

`sanity init` writes the `projectId`/`dataset` into the CLI config. Alternatively,
set them via environment variables (read by `sanity.config.ts` / `sanity.cli.ts`):

```bash
export SANITY_STUDIO_PROJECT_ID=<your-project-id>
export SANITY_STUDIO_DATASET=production
```

## Commands

```bash
pnpm dev      # run the Studio locally at http://localhost:3333
pnpm build    # build the Studio for production
pnpm deploy   # deploy to <project>.sanity.studio
```

## Project structure

```
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
