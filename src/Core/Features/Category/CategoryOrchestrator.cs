using System.Collections.Generic;
using System.Linq;
using Core.Features.BlogPost;
using Core.Features.BlogPost.Models;
using Core.Features.Category.Models;

namespace Core.Features.Category
{
    public class CategoryOrchestrator : ICategoryOrchestrator
    {
        private readonly IBlogPostLoader _blogPostLoader;
        private readonly ICategoryLoader _categoryLoader;

        public CategoryOrchestrator(
            IBlogPostLoader blogPostLoader,
            ICategoryLoader categoryLoader
        )
        {
            _categoryLoader = categoryLoader;
            _blogPostLoader = blogPostLoader;
        }

        public IEnumerable<CategoryViewModel> GetCategories(string id, out string title)
        {
            if (string.IsNullOrEmpty(id))
            {
                title = "Categories";

                return _categoryLoader
                    .GetCategories()
                    .Result
                    .Select(_ => new CategoryViewModel(_));
            }

            var categoryContent = _categoryLoader
                .GetCategory(id)
                .Result;

            title = $"Category: {categoryContent?.Title ?? id}";

            return categoryContent == null
                ? Enumerable.Empty<CategoryViewModel>()
                : new CategoryViewModel[] { new(categoryContent) };
        }

        public IEnumerable<BlogPostViewModel> GetBlogPosts(string categoryId)
        {
            var blogPostContents = _blogPostLoader
                                       .GetBlogPostsWithCategory(categoryId)
                                       .Result;

            return blogPostContents == null
                ? Enumerable.Empty<BlogPostViewModel>()
                : blogPostContents.Select(_ => new BlogPostViewModel(_));
        }
    }
}
