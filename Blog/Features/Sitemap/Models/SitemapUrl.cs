namespace Blog.Features.Sitemap.Models;

public class SitemapUrl
{
    // Relative path, e.g. "/blogpost/my-post" or "/".
    public string Path { get; set; }
    public DateTime? LastModified { get; set; }
}
