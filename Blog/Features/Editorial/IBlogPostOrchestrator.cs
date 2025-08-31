namespace Blog.Features.Editorial;

public interface IBlogPostOrchestrator
{
    public Task<IEnumerable<BlogPostViewModel>> GetBlogPosts(string id, bool preview);
}