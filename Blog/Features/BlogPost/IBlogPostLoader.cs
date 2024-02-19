namespace Blog.Features.BlogPost;

public interface IBlogPostLoader
{
    Task<BlogPostContent> Get(string slug);
    Task<string> GetSlug(string id);
    Task<BlogPostContent> GetPreview(string id);
    Task<IEnumerable<BlogPostContent>> Get(int take);
    Task<IEnumerable<BlogPostContent>> GetWithCategory(string categorySlug);
}