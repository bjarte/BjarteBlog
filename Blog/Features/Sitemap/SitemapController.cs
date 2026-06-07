namespace Blog.Features.Sitemap;

[ApiController]
public class SitemapController(ISitemapOrchestrator orchestrator) : ControllerBase
{
    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> Get()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var urls = await orchestrator.GetUrls();

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(ns + "urlset",
                urls.Select(url => new XElement(ns + "url",
                    new XElement(ns + "loc", baseUrl + url.Path),
                    url.LastModified.HasValue
                        ? new XElement(ns + "lastmod", url.LastModified.Value.ToString("yyyy-MM-dd"))
                        : null))));

        return Content(doc.Declaration + Environment.NewLine + doc, "application/xml", Encoding.UTF8);
    }
}
