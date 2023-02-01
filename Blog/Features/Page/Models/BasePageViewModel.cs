namespace Blog.Features.Page.Models;

public abstract class BasePageViewModel
{
    public string Title { get; set; }

    public string Slug { get; set; }

    public string Description { get; set; }

    public string CanonicalUrl { get; set; }
}