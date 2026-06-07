namespace Blog.Features.Editorial;

public interface IPageLoader
{
    Task<PageContent> Get(string slug);
    Task<string> GetSlug(string id);
}