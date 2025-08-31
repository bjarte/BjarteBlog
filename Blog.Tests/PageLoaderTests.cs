using System;
using System.Threading;
using System.Threading.Tasks;
using Blog.Features.Contentful;
using Blog.Features.Page;
using Blog.Features.Page.Models;
using Blog.Features.Renderers;
using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Blog.Tests;

public class PageLoaderTests
{
    [Fact]
    public async Task Get_ReturnsCachedPage_WhenCacheHit()
    {
        // Arrange
        const string slug = "test-slug";
        var page = new PageContent { Slug = slug, Title = "Cached" };

        var memoryCache = new MemoryCache(new MemoryCacheOptions());

        memoryCache.Set(slug, page, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(10)
        });

        var contentfulClientMock = new Mock<IContentfulClient>();
        var richTextRendererMock = new Mock<IRichTextRenderer>();
        var optionsMock = new Mock<IOptions<ContentfulConfig>>();
        optionsMock.Setup(o => o.Value).Returns(new ContentfulConfig());

        var loader = new PageLoader(
            optionsMock.Object,
            contentfulClientMock.Object,
            richTextRendererMock.Object,
            memoryCache
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

        var optionsMock = new Mock<IOptions<ContentfulConfig>>();
        optionsMock.Setup(o => o.Value).Returns(new ContentfulConfig());

        var loader = new PageLoader(
            optionsMock.Object,
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