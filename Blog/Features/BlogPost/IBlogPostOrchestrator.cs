using Blog.Features.BlogPost.Models;

namespace Blog.Features.BlogPost;

public interface IBlogPostOrchestrator
{
    public IEnumerable<BlogPostViewModel> GetBlogPosts(string id, bool preview, out string title);

}