namespace Blog.Features.BlogPost;

public interface IBlogPostOrchestrator
{
    public Task<IEnumerable<BlogPostViewModel>> GetBlogPosts(string id, bool preview);
}