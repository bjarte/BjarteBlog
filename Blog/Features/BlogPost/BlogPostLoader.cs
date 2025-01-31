using Contentful.Core.Errors;

namespace Blog.Features.BlogPost;

public class BlogPostLoader : IBlogPostLoader
{
    private const string BlogPostContentType = "blogpost";

    private readonly IContentfulClient _contentDeliveryClient;
    private readonly ContentfulClient _previewClient;
    private readonly IRichTextRenderer _richTextRenderer;
    private readonly string _orderNewestFirst;

    public BlogPostLoader(
        IOptions<ContentfulConfig> contentfulConfig,
        IContentfulClient contentDeliveryClient,
        IRichTextRenderer richTextRenderer
    )
    {
        _contentDeliveryClient = contentDeliveryClient;

        var contentfulOptions = contentfulConfig.Value.ToContentfulOptions();
        contentfulOptions.UsePreviewApi = true;
        _previewClient = new ContentfulClient(new HttpClient(), contentfulOptions);

        _orderNewestFirst = SortOrderBuilder<BlogPostContent>
            .New(content => content.PublishedAt, SortOrder.Reversed)
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
            .ContentTypeIs(BlogPostContentType)
            .FieldEquals(content => content.Slug, slug)
            .Include(2);

        try
        {
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

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(BlogPostContentType)
            .FieldEquals(content => content.Sys.Id, id)
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
            .ContentTypeIs(BlogPostContentType)
            .FieldEquals(content => content.Sys.Id, id)
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
            .ContentTypeIs(BlogPostContentType)
            .FieldEquals(content => content.IncludeInSearchAndNavigation, "true")
            .Include(4)
            .OrderBy(_orderNewestFirst);

        if (take > 0)
        {
            query = query.Limit(take);
        }

        try
        {
            return await _contentDeliveryClient
                .GetEntries(query);
        }
        catch (ContentfulException)
        {
            // Cannot access Contentful
            return [];
        }
    }

    public async Task<IEnumerable<BlogPostContent>> GetWithCategory(string categorySlug)
    {
        if (string.IsNullOrEmpty(categorySlug))
        {
            return [];
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(BlogPostContentType)
            .FieldEquals(content => content.IncludeInSearchAndNavigation, "true")
            .OrderBy(_orderNewestFirst);

        try
        {
            var blogPosts = await _contentDeliveryClient
                .GetEntries(query);

            return blogPosts
                .Where(blogPost => blogPost.Categories != null
                                   && blogPost
                                       .Categories
                                       .Select(categoryContent => categoryContent.Slug)
                                       .Contains(categorySlug)
                );
            ;
        }
        catch (ContentfulException)
        {
            return [];
        }
    }
}