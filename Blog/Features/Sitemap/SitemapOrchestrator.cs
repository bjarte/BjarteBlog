namespace Blog.Features.Sitemap;

public class SitemapOrchestrator(
    IBlogPostLoader blogPostLoader,
    ICategoryLoader categoryLoader,
    IPageLoader pageLoader
) : ISitemapOrchestrator
{
    public async Task<IEnumerable<SitemapUrl>> GetUrls()
    {
        var blogPosts = (await blogPostLoader.Get()).ToList();
        var categories = (await categoryLoader.Get()).ToList();
        var aboutMePage = await pageLoader.Get(PageConstants.AboutMeSlug);

        var newestBlogPostDate = blogPosts
            .Select(blogPost => blogPost.PublishedAt ?? blogPost.Sys?.UpdatedAt)
            .Where(date => date.HasValue)
            .DefaultIfEmpty(null)
            .Max();

        var newestCategoryDate = categories
            .Select(category => category.Sys?.UpdatedAt)
            .Where(date => date.HasValue)
            .DefaultIfEmpty(null)
            .Max();

        var urls = new List<SitemapUrl>
        {
            new() { Path = "/", LastModified = newestBlogPostDate }
        };

        if (aboutMePage?.Slug != null)
        {
            urls.Add(new SitemapUrl
            {
                Path = $"/page/{aboutMePage.Slug}",
                LastModified = aboutMePage.Sys?.UpdatedAt
            });
        }

        urls.AddRange(blogPosts
            .Where(blogPost => !string.IsNullOrEmpty(blogPost.Slug))
            .Select(blogPost => new SitemapUrl
            {
                Path = $"/blogpost/{blogPost.Slug}",
                LastModified = blogPost.Sys?.UpdatedAt ?? blogPost.PublishedAt
            }));
        urls.Add(new SitemapUrl { Path = "/blogpost", LastModified = newestBlogPostDate });

        // Categories (alphabetical), then the /category listing page.
        urls.AddRange(categories
            .Where(category => !string.IsNullOrEmpty(category.Slug))
            .Select(category => new SitemapUrl
            {
                Path = $"/category/{category.Slug}",
                LastModified = category.Sys?.UpdatedAt
            }));
        urls.Add(new SitemapUrl { Path = "/category", LastModified = newestCategoryDate });

        return urls;
    }
}
