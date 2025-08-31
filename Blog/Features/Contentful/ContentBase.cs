namespace Blog.Features.Contentful;

public abstract class ContentBase : IContent
{
    public SystemProperties Sys { get; set; }

    public string Title { get; set; }
    public string Slug { get; set; }
}