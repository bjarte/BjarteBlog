namespace Blog.Features.Robots;

[ApiController]
public class RobotsController : ControllerBase
{
    [HttpGet("/robots.txt")]
    public IActionResult Get()
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var robots = new StringBuilder()
            .AppendLine("User-agent: *")
            .AppendLine("Allow: /")
            .AppendLine()
            .AppendLine($"Sitemap: {baseUrl}/sitemap.xml")
            .ToString();

        return Content(robots, "text/plain", Encoding.UTF8);
    }
}
