namespace Blog.Features.Sitemap;

public class SitemapOrchestrator(
    IBlogPostLoader blogPostLoader,
    ICategoryLoader categoryLoader,
    IPageLoader pageLoader
) : ISitemapOrchestrator
{
    public async Task<IEnumerable<SitemapUrl>> GetUrls()
    {
        // Loaders already order their results: blog posts newest first,
        // categories alphabetically. Pages are unordered here.
        var blogPosts = (await blogPostLoader.Get()).ToList();
        var categories = (await categoryLoader.Get()).ToList();
        var pages = (await pageLoader.Get()).ToList();

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

        // The about-me page comes first, right after the home page.
        var aboutMe = pages.FirstOrDefault(page =>
            string.Equals(page.Slug, PageConstants.AboutMeSlug, StringComparison.OrdinalIgnoreCase));
        if (aboutMe != null)
        {
            urls.Add(new SitemapUrl
            {
                Path = $"/page/{aboutMe.Slug}",
                LastModified = aboutMe.Sys?.UpdatedAt
            });
        }

        // Blog posts (newest first), then the /blogpost listing page.
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

        // Remaining pages, alphabetically by title, excluding about-me (added above).
        urls.AddRange(pages
            .Where(page => !string.IsNullOrEmpty(page.Slug)
                           && !string.Equals(page.Slug, PageConstants.AboutMeSlug, StringComparison.OrdinalIgnoreCase))
            .OrderBy(page => page.Title)
            .Select(page => new SitemapUrl
            {
                Path = $"/page/{page.Slug}",
                LastModified = page.Sys?.UpdatedAt
            }));

        return urls;
    }
}
