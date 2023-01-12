using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
using Core.Features.AppSettings;
using Core.Features.BlogPost.Models;
using Core.Features.Editorial;

namespace Core.Features.BlogPost;

public class BlogPostLoader : IBlogPostLoader
{
    private readonly IContentfulClient _contentDeliveryClient;
    private readonly IContentfulClient _previewClient;
    private readonly string _orderNewestFirst;

    public BlogPostLoader(
        IAppSettingsService appSettingsService,
        IContentfulClient contentDeliveryClient
    )
    {
        _contentDeliveryClient = contentDeliveryClient;

        var options = appSettingsService.GetContentfulOptions();
        options.UsePreviewApi = true;

        _previewClient = new ContentfulClient(new HttpClient(), options);

        _orderNewestFirst = SortOrderBuilder<BlogPostContent>
            .New(_ => _.PublishedAt, SortOrder.Reversed)
            .Build();
    }

    public async Task<BlogPostContent> GetBlogPost(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs("blogpost")
            .FieldEquals(_ => _.Slug, slug)
            .Include(2);

        var blogPosts = await _contentDeliveryClient
            .GetEntries(query);

        var blogPost = blogPosts.FirstOrDefault();

        blogPost.BodyToHtml();
        return blogPost;
    }

    public async Task<BlogPostContent> GetBlogPostPreview(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs("blogpost")
            .FieldEquals(_ => _.Sys.Id, id)
            .Include(2);

        var blogPosts = await _previewClient
            .GetEntries(query);

        var blogPost = blogPosts.FirstOrDefault();

        blogPost.BodyToHtml();
        return blogPost;
    }

    public async Task<IEnumerable<BlogPostContent>> GetBlogPosts()
    {
        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs("blogpost")
            .FieldEquals(_ => _.IncludeInSearchAndNavigation, "true")
            .Include(4)
            .OrderBy(_orderNewestFirst);

        return await _contentDeliveryClient
             .GetEntries(query);
    }

    public async Task<IEnumerable<BlogPostContent>> GetBlogPostsWithCategory(string categorySlug)
    {
        if (string.IsNullOrEmpty(categorySlug))
        {
            return Enumerable.Empty<BlogPostContent>();
        }

        var query = new QueryBuilder<BlogPostContent>()
            .ContentTypeIs("blogpost")
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