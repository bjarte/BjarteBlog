using Blog.Features.BlogPost.Models;
using Blog.Features.Category.Models;

namespace Blog.Features.Category;

public interface ICategoryOrchestrator
{
    public IEnumerable<CategoryViewModel> GetCategories(string id, out string title);
    public IEnumerable<BlogPostViewModel> GetBlogPosts(string categoryId);
}