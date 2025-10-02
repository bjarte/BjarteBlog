namespace Blog.Tests;

public class PageLoaderTests
{
    [Fact]
    public async Task Get_ReturnsCachedPage_WhenCacheHit()
    {
        // Arrange
        const string slug = "test-slug";
        const string cacheKey = $"contentful_page_{slug}";
        var page = new PageContent { Slug = slug, Title = "Cached" };

        var cache = new MemoryCache(new MemoryCacheOptions());

        cache.Set(cacheKey, page, MemoryCacheConstants.SlidingExpiration1Day);

        var contentfulClientMock = new Mock<IContentfulClient>();
        var richTextRendererMock = new Mock<IRichTextRenderer>();

        var loader = new PageLoader(
            contentfulClientMock.Object,
            richTextRendererMock.Object,
            cache
        );

        // Act
        var result = await loader.Get(slug);

        // Assert
        Assert.Equal(page, result);
        contentfulClientMock.Verify(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<PageContent>>(), CancellationToken.None), Times.Never);
    }

    [Fact]
    public async Task Get_CachesPage_WhenCacheMiss()
    {
        // Arrange
        const string slug = "test-slug";
        var page = new PageContent { Slug = slug, Title = "FromContentful" };
        var pageCollection = new ContentfulCollection<PageContent> { Items = [page] };

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        var contentfulClientMock = new Mock<IContentfulClient>();
        contentfulClientMock.Setup(c => c.GetEntries(It.IsAny<QueryBuilder<PageContent>>(), CancellationToken.None)).ReturnsAsync(pageCollection);

        var richTextRendererMock = new Mock<IRichTextRenderer>();
        richTextRendererMock.Setup(r => r.BodyToHtml(page)).Returns("html");

        var loader = new PageLoader(
            contentfulClientMock.Object,
            richTextRendererMock.Object,
            memoryCache
        );

        // Act
        var result = await loader.Get(slug);

        // Assert
        Assert.Equal(page, result);
        contentfulClientMock.Verify(c => c.GetEntries(It.IsAny<QueryBuilder<PageContent>>(), CancellationToken.None), Times.Once);
    }
}