namespace Blog.Features.Editorial;

public class BlogPostLoader(
    IContentfulClient contentDeliveryClient,
    IRichTextRenderer richTextRenderer,
    IMemoryCache cache
) : IBlogPostLoader
{
    private readonly string _orderNewestFirst = SortOrderBuilder<BlogPostContent>
        .New(content => content.PublishedAt, SortOrder.Reversed)
        .Build();

    public async Task<BlogPostContent> Get(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var cacheKey = $"contentful_blogpost_{slug}";
        if (cache.TryGetValue(cacheKey, out BlogPostContent cachedBlogPost))
        {
            return cachedBlogPost;
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(ContentTypes.BlogPost)
            .FieldEquals(content => content.Slug, slug)
            .Include(2);

        try
        {
            var blogPost = (await contentDeliveryClient
                    .GetEntries(query))
                .FirstOrDefault();

            if (blogPost == null)
            {
                return null;
            }

            blogPost.BodyString = richTextRenderer.BodyToHtml(blogPost);

            cache.Set(cacheKey, blogPost, MemoryCacheConstants.SlidingExpiration1Day);

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
        if (cache.TryGetValue(cacheKey, out string cachedSlug))
        {
            return cachedSlug;
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(ContentTypes.BlogPost)
            .FieldEquals(content => content.Sys.Id, id);

        try
        {
            var slug = (await contentDeliveryClient
                    .GetEntries(query))
                .FirstOrDefault()?
                .Slug;

            cache.Set(cacheKey, slug, MemoryCacheConstants.SlidingExpiration1Day);

            return slug;
        }
        catch (ContentfulException)
        {
            return null;
        }
    }

    public async Task<IEnumerable<BlogPostContent>> Get(int take = 0)
    {
        var cacheKey = $"contentful_all_blogposts_{take}";
        if (cache.TryGetValue(cacheKey, out IEnumerable<BlogPostContent> cachedBlogPosts))
        {
            return cachedBlogPosts;
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs(ContentTypes.BlogPost)
            .FieldEquals(content => content.IncludeInSearchAndNavigation, "true")
            .Include(4)
            .OrderBy(_orderNewestFirst);

        if (take > 0)
        {
            query = query.Limit(take);
        }

        try
        {
            var blogPosts = await contentDeliveryClient
                .GetEntries(query);

            cache.Set(cacheKey, blogPosts, MemoryCacheConstants.SlidingExpiration1Day);

            return blogPosts;
        }
        catch (ContentfulException)
        {
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
        if (cache.TryGetValue(cacheKey, out IEnumerable<BlogPostContent> cachedBlogPosts))
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

        cache.Set(cacheKey, blogPostsWithCategory, MemoryCacheConstants.SlidingExpiration1Day);

        return blogPostsWithCategory;
    }
}