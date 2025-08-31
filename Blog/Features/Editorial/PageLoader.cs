using Contentful.Core.Errors;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Features.Editorial;

// The Content Delivery API (CDA) is a read-only API for
// retrieving content from Contentful. All content, both JSON
// and binary, is fetched from the server closest to a user's
// location by using our global CDN.
public class PageLoader(
    IContentfulClient contentDeliveryClient,
    IRichTextRenderer richTextRenderer,
    IMemoryCache cache)
    : IPageLoader
{
    public async Task<PageContent> Get(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var cacheKey = $"contentful_page_{slug}";
        if (cache.TryGetValue(cacheKey, out PageContent cachedPage))
        {
            return cachedPage;
        }

        var query = new QueryBuilder<PageContent>()
            .ContentTypeIs(ContentTypes.Page)
            .FieldEquals(content => content.Slug, slug)
            // How many levels of references do we need? Default is 1,
            // the Page object itself. Set this to 2 to include the
            // referenced Image object.
            .Include(2);

        try
        {
            var page = (await contentDeliveryClient
                    .GetEntries(query))
                .FirstOrDefault();

            if (page == null)
            {
                return null;
            }

            page.BodyString = richTextRenderer.BodyToHtml(page);

            cache.Set(cacheKey, page);

            return page;
        }
        catch (ContentfulException)
        {
            return null;
        }
    }

    public async Task<string> GetSlug(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        var cacheKey = $"contentful_page_slug_{id}";
        if (cache.TryGetValue(cacheKey, out string cachedSlug))
        {
            return cachedSlug;
        }

        var query = new QueryBuilder<PageContent>()
            .ContentTypeIs(ContentTypes.Page)
            .FieldEquals(content => content.Sys.Id, id);

        try
        {
            var slug = (await contentDeliveryClient
                    .GetEntries(query))
                .FirstOrDefault()?
                .Slug;

            if (!string.IsNullOrWhiteSpace(slug))
            {
                cache.Set(cacheKey, slug);
            }

            return slug;
        }
        catch (ContentfulException)
        {
            return null;
        }
    }
}