# Bjarte's Blog

Blog available at <https://basementmedia.no>

The blog is made up of a .NET 10 web application and Contentful as the content source.

To run the main site:
> dotnet watch run --project .\Blog\Blog.csproj

To run all tests:
> dotnet test

## Sanity Studio (in progress)

The `Sanity/` directory holds a standalone [Sanity](https://www.sanity.io/) Studio (Sanity v6,
TypeScript). It is the first step in migrating the content source from Contentful to Sanity; the
.NET app is **not** wired to it yet.

Document types: **Blog Post** (copied from the Contentful model, with a Portable Text body),
**Category**, **Image**, and **Page**.

To run the Studio locally:
> cd Sanity
>
> pnpm install
>
> pnpm dev   # serves at http://localhost:3333

It connects to a Sanity project via `SANITY_STUDIO_PROJECT_ID` / `SANITY_STUDIO_DATASET`, set in
`Sanity/.env` (gitignored). See `Sanity/README.md` for setup details.
