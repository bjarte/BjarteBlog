using Blog.Features.BlogPost;
using Blog.Features.BlogPost.Models;
using Blog.Features.Category.Models;

namespace Blog.Features.Category;

public class CategoryOrchestrator(IBlogPostLoader blogPostLoader, ICategoryLoader categoryLoader) : ICategoryOrchestrator
{
    public IEnumerable<CategoryViewModel> GetCategories(string id, out string title)
    {
        if (string.IsNullOrEmpty(id))
        {
            title = "Categories";

            return categoryLoader
                .Get()
                .Result
                .Select(_ => new CategoryViewModel(_));
        }

        var categoryContent = categoryLoader
            .Get(id)
            .Result;

        title = $"Category: {categoryContent?.Title ?? id}";

        return categoryContent == null
            ? Enumerable.Empty<CategoryViewModel>()
            : new CategoryViewModel[] { new(categoryContent) };
    }

    public IEnumerable<BlogPostViewModel> GetBlogPosts(string categoryId)
    {
        var blogPostContents = blogPostLoader
            .GetWithCategory(categoryId)
            .Result;

        return blogPostContents == null
            ? Enumerable.Empty<BlogPostViewModel>()
            : blogPostContents.Select(_ => new BlogPostViewModel(_));
    }
}