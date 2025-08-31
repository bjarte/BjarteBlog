using Contentful.Core.Errors;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Features.BlogPost;

public class BlogPostLoader : IBlogPostLoader
{
    private const string BlogPostContentType = "blogpost";

    private readonly IContentfulClient _contentDeliveryClient;
    private readonly ContentfulClient _previewClient;
    private readonly IRichTextRenderer _richTextRenderer;
    private readonly IMemoryCache _cache;

    private readonly string _orderNewestFirst = SortOrderBuilder<BlogPostContent>
        .New(content => content.PublishedAt, SortOrder.Reversed)
        .Build();

    public BlogPostLoader(
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

    public async Task<BlogPostContent> Get(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var cacheKey = $"contentful_blogpost_{slug}";
        if (_cache.TryGetValue(cacheKey, out BlogPostContent cachedBlogPost))
        {
            return cachedBlogPost;
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

            _cache.Set(cacheKey, blogPost);

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

        var cacheKey = $"contentful_blogpost_slug_{id}";
        if (_cache.TryGetValue(cacheKey, out string cachedSlug))
        {
            return cachedSlug;
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(BlogPostContentType)
            .FieldEquals(content => content.Sys.Id, id)
            .Include(1);

        var pages = await _contentDeliveryClient
            .GetEntries(query);

        var slug = pages.FirstOrDefault()?.Slug;

        if (!string.IsNullOrWhiteSpace(slug))
        {
            _cache.Set(cacheKey, slug);
        }

        return slug;
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
        var cacheKey = $"contentful_all_blogposts_{take}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<BlogPostContent> cachedBlogPosts))
        {
            return cachedBlogPosts;
        }

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
            var blogPosts = await _contentDeliveryClient
                .GetEntries(query);

            _cache.Set(cacheKey, blogPosts);

            return blogPosts;
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

        var cacheKey = $"contentful_blogposts_with_category_{categorySlug}";
        if (_cache.TryGetValue(cacheKey, out IEnumerable<BlogPostContent> cachedBlogPosts))
        {
            return cachedBlogPosts;
        }

        var blogPosts = await Get();

        var blogPostsWithCategory = blogPosts
            .Where(blogPost => blogPost.Categories != null
                               && blogPost
                                   .Categories
                                   .Select(categoryContent => categoryContent.Slug)
                                   .Contains(categorySlug)
            )
            .ToList();

        _cache.Set(cacheKey, blogPostsWithCategory);

        return blogPostsWithCategory;
    }
}