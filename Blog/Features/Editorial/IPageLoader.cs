namespace Blog.Features.Editorial;

public interface IPageLoader
{
    Task<PageContent> Get(string slug);
    Task<IEnumerable<PageContent>> Get();
    Task<string> GetSlug(string id);
}