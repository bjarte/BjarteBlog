namespace Blog.Features.Category;

public interface ICategoryOrchestrator
{
    public Task<List<CategoryViewModel>> GetCategories(string id);
    public Task<IEnumerable<BlogPostViewModel>> GetBlogPosts(string categoryId);
}