namespace Blog.Tests;

public class SitemapOrchestratorTests
{
    [Fact]
    public async Task GetUrls_ReturnsHomeBlogPostsCategoriesAndPages_InOrder()
    {
        // Arrange
        // Loaders order blog posts newest-first and categories alphabetically.
        var blogPosts = new List<BlogPostContent>
        {
            new()
            {
                Slug = "newest-post",
                PublishedAt = new DateTime(2024, 03, 15),
                Sys = new SystemProperties { UpdatedAt = new DateTime(2024, 03, 20) }
            },
            new()
            {
                Slug = "second-post",
                PublishedAt = new DateTime(2024, 01, 10),
                Sys = new SystemProperties { UpdatedAt = new DateTime(2024, 02, 01) }
            }
        };

        var categories = new List<CategoryContent>
        {
            new() { Slug = "architecture", Sys = new SystemProperties { UpdatedAt = new DateTime(2024, 04, 01) } },
            new() { Slug = "dotnet", Sys = new SystemProperties { UpdatedAt = new DateTime(2024, 03, 25) } }
        };

        var aboutMePage = new PageContent
        {
            Slug = PageConstants.AboutMeSlug,
            Title = "About me",
            Sys = new SystemProperties
            {
                UpdatedAt = new DateTime(2024, 05, 01)
            }
        };

        var blogPostLoaderMock = new Mock<IBlogPostLoader>();
        blogPostLoaderMock.Setup(loader => loader.Get(0)).ReturnsAsync(blogPosts);

        var categoryLoaderMock = new Mock<ICategoryLoader>();
        categoryLoaderMock.Setup(loader => loader.Get()).ReturnsAsync(categories);

        var pageLoaderMock = new Mock<IPageLoader>();
        pageLoaderMock.Setup(loader => loader.Get(It.IsAny<string>())).ReturnsAsync(aboutMePage);

        var orchestrator = new SitemapOrchestrator(
            blogPostLoaderMock.Object,
            categoryLoaderMock.Object,
            pageLoaderMock.Object);

        // Act
        var result = (await orchestrator.GetUrls()).ToList();

        // Assert
        Assert.Collection(result.Select(url => url.Path),
            path => Assert.Equal("/", path),
            path => Assert.Equal("/page/about-me", path),
            path => Assert.Equal("/blogpost/newest-post", path),
            path => Assert.Equal("/blogpost/second-post", path),
            path => Assert.Equal("/blogpost", path),
            path => Assert.Equal("/category/architecture", path),
            path => Assert.Equal("/category/dotnet", path),
            path => Assert.Equal("/category", path));

        // Home and the /blogpost listing share the newest blog post date.
        Assert.Equal(new DateTime(2024, 03, 15), result[0].LastModified);
        Assert.Equal(new DateTime(2024, 03, 15), result[4].LastModified);
        // about-me lastmod.
        Assert.Equal(new DateTime(2024, 05, 01), result[1].LastModified);
        // Blog post lastmod prefers Sys.UpdatedAt.
        Assert.Equal(new DateTime(2024, 03, 20), result[2].LastModified);
        // The /category listing uses the newest category date.
        Assert.Equal(new DateTime(2024, 04, 01), result[7].LastModified);
    }

    [Fact]
    public async Task GetUrls_SkipsEntriesWithEmptySlug()
    {
        // Arrange
        var blogPostLoaderMock = new Mock<IBlogPostLoader>();
        blogPostLoaderMock.Setup(loader => loader.Get(0))
            .ReturnsAsync(new List<BlogPostContent> { new() { Slug = "" } });

        var categoryLoaderMock = new Mock<ICategoryLoader>();
        categoryLoaderMock.Setup(loader => loader.Get())
            .ReturnsAsync(new List<CategoryContent> { new() { Slug = null } });

        var pageLoaderMock = new Mock<IPageLoader>();
        pageLoaderMock.Setup(loader => loader.Get(It.IsAny<string>()))
            .ReturnsAsync(new PageContent());

        var orchestrator = new SitemapOrchestrator(
            blogPostLoaderMock.Object,
            categoryLoaderMock.Object,
            pageLoaderMock.Object);

        // Act
        var result = (await orchestrator.GetUrls()).ToList();

        // Assert: only the home and listing entries remain; the slug-less entries are skipped.
        Assert.Collection(result.Select(url => url.Path),
            path => Assert.Equal("/", path),
            path => Assert.Equal("/blogpost", path),
            path => Assert.Equal("/category", path));
    }
}
