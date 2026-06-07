namespace Blog.Features.Sitemap;

[ApiController]
public class SitemapController(
    IOptions<SitemapConfig> sitemapConfig,
    ISitemapOrchestrator sitemapOrchestrator
    ) : ControllerBase
{
    private readonly string _baseUrl = sitemapConfig.Value.BaseUrl;

    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> Get()
    {
        var urls = await sitemapOrchestrator.GetUrls();

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(ns + "urlset",
                urls.Select(url => new XElement(ns + "url",
                    new XElement(ns + "loc", _baseUrl + url.Path),
                    url.LastModified.HasValue
                        ? new XElement(ns + "lastmod", url.LastModified.Value.ToString("yyyy-MM-dd"))
                        : null))));

        return Content(doc.Declaration + Environment.NewLine + doc, "application/xml", Encoding.UTF8);
    }
}
