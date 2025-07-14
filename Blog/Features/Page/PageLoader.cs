using Contentful.Core.Errors;

namespace Blog.Features.Page;

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

    public PageLoader(
        IOptions<ContentfulConfig> contentfulConfig,
        IContentfulClient contentDeliveryClient,
        IRichTextRenderer richTextRenderer
    )
    {
        _contentDeliveryClient = contentDeliveryClient;

        var contentfulOptions = contentfulConfig.Value.ToContentfulOptions();
        contentfulOptions.UsePreviewApi = true;

        _previewClient = new ContentfulClient(new HttpClient(), contentfulOptions);

        _richTextRenderer = richTextRenderer;
    }

    public async Task<PageContent> Get(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
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
            var pages = await _contentDeliveryClient
                .GetEntries(query);

            var page = pages.FirstOrDefault();
            if (page == null)
            {
                return null;
            }

            page.BodyString = _richTextRenderer.BodyToHtml(page);
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

        var query = new QueryBuilder<PageContent>()
            .ContentTypeIs(PageContentType)
            .FieldEquals(content => content.Sys.Id, id)
            .Include(1);

        var pages = await _contentDeliveryClient
            .GetEntries(query);

        return pages.FirstOrDefault()?.Slug;
    }

    public async Task<PageContent> GetPreview(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return null;
        }

        var query = new QueryBuilder<PageContent>()
            .ContentTypeIs(PageContentType)
            .FieldEquals(content => content.Sys.Id, id)
            .Include(2);

        var pages = await _previewClient
            .GetEntries(query);

        var page = pages.FirstOrDefault();
        if (page == null)
        {
            return null;
        }

        page.BodyString = _richTextRenderer.BodyToHtml(page);
        return page;
    }
}