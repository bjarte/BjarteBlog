namespace Blog.Features.Robots;

[ApiController]
public class RobotsApiController(
    IOptions<SitemapConfig> sitemapConfig
) : ControllerBase
{
    private readonly string _baseUrl = sitemapConfig.Value.BaseUrl;

    [HttpGet("/robots.txt")]
    public IActionResult Get()
    {
        var robots = new StringBuilder()
            .AppendLine("User-agent: *")
            .AppendLine("Allow: /")
            .AppendLine()
            .AppendLine($"Sitemap: {_baseUrl}/sitemap.xml")
            .ToString();

        return Content(robots, "text/plain", Encoding.UTF8);
    }
}
