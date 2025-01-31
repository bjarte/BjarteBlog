namespace Blog.Features.Category;

public class CategoryOrchestrator(IBlogPostLoader blogPostLoader, ICategoryLoader categoryLoader) : ICategoryOrchestrator
{
    public async Task<List<CategoryViewModel>> GetCategories(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            var categoryContents = await categoryLoader.Get();

            return categoryContents
                .Select(content => new CategoryViewModel(content))
                .ToList();
        }

        var categoryContent = await categoryLoader.Get(id);

        return categoryContent == null
            ? []
            : [new CategoryViewModel(categoryContent)];
    }

    public async Task<IEnumerable<BlogPostViewModel>> GetBlogPosts(string categoryId)
    {
        var blogPostContents = await blogPostLoader
            .GetWithCategory(categoryId);

        return blogPostContents == null
            ? []
            : blogPostContents.Select(content => new BlogPostViewModel(content));
    }
}