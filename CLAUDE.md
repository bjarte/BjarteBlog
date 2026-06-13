# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A personal blog (<https://basementmedia.no>) built as an ASP.NET Core Razor Pages app that pulls all content from **Contentful** (headless CMS). There is no database — Contentful is the single source of truth, fronted by an in-memory cache. Deployed to Azure via Azure DevOps pipeline.

Note: the project targets **net10.0**.

A migration off Contentful to **Sanity** is in progress. The `Sanity/` directory holds a standalone Sanity Studio (the authoring UI). The .NET app in `Blog/` still reads exclusively from Contentful — it is not yet wired to Sanity. See [The Sanity Studio](#the-sanity-studio-sanity) below.

## Commands

```powershell
# Run the site with hot reload
dotnet watch run --project .\Blog\Blog.csproj

# Run all tests
dotnet test

# Run a single test by name
dotnet test --filter "FullyQualifiedName~PageLoaderTests.Get_ReturnsCachedPage_WhenCacheHit"

# Run all tests in one class
dotnet test --filter "FullyQualifiedName~PageLoaderTests"

# Build
dotnet build
```

Local dev runs at `https://localhost:44330`.

## Configuration & Secrets

Contentful credentials bind from the `ContentfulOptions` section (`ContentfulConfig`, all `[Required]`). `appsettings.json` ships with empty values:

- **Locally**: provide them via .NET User Secrets (`UserSecretsId` is in `Blog.csproj`). See `docs/HowToStoreUserSecrets.md`.
- **Staging/Production**: injected as Azure DevOps pipeline variables, applied via the `FileTransform` task in `azure-pipelines.yml`.

## Architecture

### Feature folders

Code is organized by feature under `Blog/Features/` (Category, CodeBlock, Editorial, Image, Navigation, Renderers, Contentful, Initialization), not by technical layer. Razor Pages live in `Blog/Pages/`.

`Blog/GlobalUsings.cs` globally imports every feature namespace plus the Contentful SDK namespaces — new files rarely need explicit `using` statements for in-project types.

### The Loader → Orchestrator → ViewModel pipeline

This is the core pattern; follow it when adding content types:

1. **Content models** (`*Content` under `Models/`) deserialize Contentful entries. Most derive from `EditorialContent` (which has a `Body` rich-text field + `BodyString` for rendered HTML) or `ContentBase` (`Sys`, `Title`, `Slug`, implements Contentful's `IContent`). Content type IDs are centralized in `ContentTypes`.
2. **Loaders** (`*Loader` implementing `I*Loader`) query Contentful via `QueryBuilder<T>` and the injected `IContentfulClient`. They own the **caching layer** (see below) and swallow `ContentfulException` by returning `null`/empty.
3. **Orchestrators** (`*Orchestrator` implementing `I*Orchestrator`) compose multiple loaders into the data a page needs (e.g. `BlogPostOrchestrator` joins a blog post with its author page; `NavigationOrchestrator` resolves internal link targets to slugs).
4. **ViewModels** (`*ViewModel`) are constructed from content models and are what Razor Pages consume. The page model (`*.cshtml.cs`) calls an orchestrator in `OnGet` and exposes ViewModels.

All loaders, orchestrators, and renderers are registered as **singletons** in `Features/Initialization/DependencyInjection.cs` (`AddDependencies`). Register any new service there. `AppsettingsSetup.cs` (`AddAppsettings`) binds the options.

### Two Contentful clients

- **Delivery (CDA)** — the standard `IContentfulClient` registered by `AddContentful` (read-only, CDN-cached published content). Used by all normal loaders.
- **Preview** — `PreviewLoader` manually constructs a separate `ContentfulClient` with `UsePreviewApi = true` to fetch unpublished drafts. Pages take a `?preview=true` query param that routes through the preview loader instead of the cache. `ContentfulConfigExtensions.ToContentfulOptions()` maps config to SDK options.

### Caching

Loaders cache results in `IMemoryCache` with a 1-day sliding expiration (`MemoryCacheConstants.SlidingExpiration1Day`). Cache keys are string-interpolated per slug/id (e.g. `$"contentful_page_{slug}"`). There is **no cache invalidation** — content updates appear after the sliding window lapses or on app restart. Preview requests bypass the cache entirely.

### Rich text rendering

Contentful rich-text `Body` is converted to HTML server-side by `RichTextRenderer.BodyToHtml`, which configures Contentful's `HtmlRenderer` with custom renderers:

- `AdvancedTextRenderer` — text formatting.
- `CodeBlockContentRenderer` — renders embedded `codeBlock` entries into `<pre><code class="language-...">` for client-side highlight.js. **Note: this renderer currently has a known broken path (see the `TODO` about `customNode.JObject`) where the embedded entry id can't be resolved.**

Rendered HTML is stored on `Content.BodyString` and emitted in views with `@Html.Raw(...)`.

## Testing

xUnit + Moq, in `Blog.Tests/`. Tests mock `IContentfulClient` and `IRichTextRenderer` and use a real `MemoryCache`; the established focus is verifying cache hit/miss behavior in loaders (see `PageLoaderTests`). `Blog.Tests/GlobalUsings.cs` mirrors the main project's global usings.

## The Sanity Studio (`Sanity/`)

A standalone **Sanity Studio v6** (TypeScript, managed with **pnpm**) living in `Sanity/`. It is
authoring-UI-only and independent of the .NET solution — there is no front-end here and the `Blog/`
app does not yet consume it. This is step one of an eventual Contentful → Sanity migration.

### Commands (run from `Sanity/`)

```powershell
pnpm install
pnpm dev      # Studio at http://localhost:3333
pnpm build    # production bundle
pnpm deploy   # deploy to <project>.sanity.studio

# The `sanity` CLI is a local dependency, not global — invoke via `npx sanity ...`
npx sanity schema validate
```

### Configuration

`sanity.config.ts` and `sanity.cli.ts` read `SANITY_STUDIO_PROJECT_ID` and `SANITY_STUDIO_DATASET`
(falling back to placeholders). Set them in `Sanity/.env` (gitignored; Vite only exposes vars
prefixed `SANITY_STUDIO_`). `Sanity/pnpm-workspace.yaml` allows `esbuild`'s install script
(required by pnpm 11+).

### Schema

Types are defined with `defineType`/`defineField` under `Sanity/schemaTypes/` and registered in
`schemaTypes/index.ts`:

- **`blogPost`** — copied field-for-field from the Contentful `blogpost` content type, except the
  rich-text `body` is **Portable Text** (`blockContent`). References `category` and `imageAsset`.
- **`category`** — copied from Contentful (`title` + `slug`).
- **`imageAsset`** — display title "Image". Named `imageAsset`, **not** `image`, to avoid shadowing
  Sanity's built-in `image` object type. Sanity-idiomatic (image with `alt`/`caption` + `credit`).
- **`page`** — Sanity-idiomatic, loosely based on the Contentful `page` type.
- **`blockContent`** — shared Portable Text type reused by `blogPost.body` and `page.body`. Mirrors
  the marks/nodes enabled on the Contentful RichText fields.
