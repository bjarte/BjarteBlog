namespace Blog.Features.Editorial;

public interface IBlogPostLoader
{
    Task<BlogPostContent> Get(string slug);
    Task<string> GetSlug(string id);
    Task<IEnumerable<BlogPostContent>> Get(int take = 0);
    Task<IEnumerable<BlogPostContent>> GetWithCategory(string categorySlug);
}