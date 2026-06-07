namespace Blog.Features.Sitemap;

public interface ISitemapOrchestrator
{
    Task<IEnumerable<SitemapUrl>> GetUrls();
}
