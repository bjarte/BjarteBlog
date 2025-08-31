using Contentful.Core.Errors;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Features.Editorial;

public class PageLoader : IPageLoader
{
    private const string PageContentType = "page";

    // The Content Delivery API (CDA) is a read-only API for
    // retrieving content from Contentful. All content, both JSON
    // and binary, is fetched from the server closest to a user's
    // location by using our global CDN.
    private readonly IContentfulClient _contentDeliveryClient;

    // The Preview API is for previewing unpublished content as though
    // it were published. It maintains the same behaviour and parameters
    // as the CDA, but delivers the latest draft for entries and assets.
    private readonly ContentfulClient _previewClient;

    // The Content Management API (CMA) is a restful API for
    // managing content in your Contentful spaces. You can create,
    // update, delete and retrieve content using well-known HTTP verbs.
    // 
    // The CMA client does not belong in the PageLoader, it should be
    // used in a separate class called the PageRepository.
    //
    //private IContentfulManagementClient _contentManagementClient;

    private readonly IRichTextRenderer _richTextRenderer;
    private readonly IMemoryCache _cache;

    public PageLoader(
        IOptions<ContentfulConfig> contentfulConfig,
        IContentfulClient contentDeliveryClient,
        IRichTextRenderer richTextRenderer,
        IMemoryCache cache
    )
    {
        _contentDeliveryClient = contentDeliveryClient;

        var contentfulOptions = contentfulConfig.Value.ToContentfulOptions();
        contentfulOptions.UsePreviewApi = true;
        _previewClient = new ContentfulClient(new HttpClient(), contentfulOptions);

        _richTextRenderer = richTextRenderer;
        _cache = cache;
    }

    public async Task<PageContent> Get(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var cacheKey = $"contentful_page_{slug}";
        if (_cache.TryGetValue(cacheKey, out PageContent cachedPage))
        {
            return cachedPage;
        }

        var query = new QueryBuilder<PageContent>()
            .ContentTypeIs(PageContentType)
            .FieldEquals(content => content.Slug, slug)
            // How many levels of references do we need? Default is 1,
            // the Page object itself. Set this to 2 to include the
            // referenced Image object.
            .Include(2);

        try
        {
            var page = (await _contentDeliveryClient
                    .GetEntries(query))
                .FirstOrDefault();

            if (page == null)
            {
                return null;
            }

            page.BodyString = _richTextRenderer.BodyToHtml(page);

            _cache.Set(cacheKey, page);

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
        if (_cache.TryGetValue(cacheKey, out string cachedSlug))
        {
            return cachedSlug;
        }

        string slug;

        try
        {
            slug = (await _contentDeliveryClient
                    .GetEntry<PageContent>(id))
                .Slug;
        }
        catch (ContentfulException)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(slug))
        {
            _cache.Set(cacheKey, slug);
        }

        return slug;
    }
}