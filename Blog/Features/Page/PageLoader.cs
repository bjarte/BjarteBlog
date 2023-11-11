using System.Net.Http;
using System.Threading.Tasks;
using Blog.Features.AppSettings;
using Blog.Features.Page.Models;
using Blog.Features.Renderers;
using Contentful.Core;
using Contentful.Core.Search;

namespace Blog.Features.Page;

// ReSharper disable once UnusedMember.Global
public class PageLoader : IPageLoader
{
    private const string pageContentType = "page";

    // The Content Delivery API (CDA) is a read-only API for
    // retrieving content from Contentful. All content, both JSON
    // and binary, is fetched from the server closest to a user's
    // location by using our global CDN.
    private readonly IContentfulClient _contentDeliveryClient;

    // The Preview API is for previewing unpublished content as though
    // it were published. It maintains the same behaviour and parameters
    // as the CDA, but delivers the latest draft for entries and assets.
    private readonly IContentfulClient _previewClient;

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
        IAppSettingsService appSettingsService,
        IContentfulClient contentDeliveryClient,
        IRichTextRenderer richTextRenderer
    )
    {
        _contentDeliveryClient = contentDeliveryClient;

        var options = appSettingsService.GetContentfulOptions();
        options.UsePreviewApi = true;

        _previewClient = new ContentfulClient(new HttpClient(), options);

        _richTextRenderer = richTextRenderer;
    }

    public async Task<PageContent> Get(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var query = new QueryBuilder<PageContent>()
            .ContentTypeIs(pageContentType)
            .FieldEquals(_ => _.Slug, slug)
            // How many levels of references do we need? Default is 1,
            // the Page object itself. Set this to 2 to include the
            // referenced Image object.
            .Include(2);

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

    public async Task<string> GetSlug(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        var query = new QueryBuilder<PageContent>()
            .ContentTypeIs(pageContentType)
            .FieldEquals(_ => _.Sys.Id, id)
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
            .ContentTypeIs(pageContentType)
            .FieldEquals(_ => _.Sys.Id, id)
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