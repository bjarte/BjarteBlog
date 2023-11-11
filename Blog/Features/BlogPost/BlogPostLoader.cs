using System.Net.Http;
using System.Threading.Tasks;
using Blog.Features.AppSettings;
using Blog.Features.BlogPost.Models;
using Blog.Features.Renderers;
using Contentful.Core;
using Contentful.Core.Search;

namespace Blog.Features.BlogPost;

public class BlogPostLoader : IBlogPostLoader
{
    private const string blogPostContentType = "blogpost";

    private readonly IContentfulClient _contentDeliveryClient;
    private readonly IContentfulClient _previewClient;
    private readonly IRichTextRenderer _richTextRenderer;
    private readonly string _orderNewestFirst;

    public BlogPostLoader(
        IAppSettingsService appSettingsService,
        IContentfulClient contentDeliveryClient,
        IRichTextRenderer richTextRenderer
    )
    {
        _contentDeliveryClient = contentDeliveryClient;

        var options = appSettingsService.GetContentfulOptions();
        options.UsePreviewApi = true;

        _previewClient = new ContentfulClient(new HttpClient(), options);

        _orderNewestFirst = SortOrderBuilder<BlogPostContent>
            .New(_ => _.PublishedAt, SortOrder.Reversed)
            .Build();

        _richTextRenderer = richTextRenderer;
    }

    public async Task<BlogPostContent> Get(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(blogPostContentType)
            .FieldEquals(_ => _.Slug, slug)
            .Include(2);

        var blogPosts = await _contentDeliveryClient
            .GetEntries(query);

        var blogPost = blogPosts.FirstOrDefault();
        if (blogPost == null)
        {
            return null;
        }

        blogPost.BodyString = _richTextRenderer.BodyToHtml(blogPost);
        return blogPost;
    }

    public async Task<string> GetSlug(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(blogPostContentType)
            .FieldEquals(_ => _.Sys.Id, id)
            .Include(1);

        var pages = await _contentDeliveryClient
            .GetEntries(query);

        return pages.FirstOrDefault()?.Slug;
    }

    public async Task<BlogPostContent> GetPreview(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(blogPostContentType)
            .FieldEquals(_ => _.Sys.Id, id)
            .Include(2);

        var blogPosts = await _previewClient
            .GetEntries(query);

        var blogPost = blogPosts.FirstOrDefault();
        if (blogPost == null)
        {
            return null;
        }

        blogPost.BodyString = _richTextRenderer.BodyToHtml(blogPost);
        return blogPost;
    }

    public async Task<IEnumerable<BlogPostContent>> Get(int take = 0)
    {
        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(blogPostContentType)
            .FieldEquals(_ => _.IncludeInSearchAndNavigation, "true")
            .Include(4)
            .OrderBy(_orderNewestFirst);

        if (take > 0)
        {
            query = query.Limit(take);
        }

        return await _contentDeliveryClient
             .GetEntries(query);
    }

    public async Task<IEnumerable<BlogPostContent>> GetWithCategory(string categorySlug)
    {
        if (string.IsNullOrEmpty(categorySlug))
        {
            return Enumerable.Empty<BlogPostContent>();
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(blogPostContentType)
            .FieldEquals(_ => _.IncludeInSearchAndNavigation, "true")
            .OrderBy(_orderNewestFirst);

        return (await _contentDeliveryClient
            .GetEntries(query))
            .Where(blogPost => blogPost.Categories != null
                && blogPost
                    .Categories
                    .Select(categoryContent => categoryContent.Slug)
                    .Contains(categorySlug)
            );
    }
}