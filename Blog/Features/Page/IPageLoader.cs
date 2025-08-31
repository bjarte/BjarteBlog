namespace Blog.Features.Page;

public interface IPageLoader
{
    Task<PageContent> Get(string slug);
    Task<string> GetSlug(string contentId);
    Task<PageContent> GetPreview(string contentId);
}