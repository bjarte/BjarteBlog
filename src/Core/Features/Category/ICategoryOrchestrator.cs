using System.Collections.Generic;
using Core.Features.BlogPost.Models;
using Core.Features.Category.Models;

namespace Core.Features.Category
{
    public interface ICategoryOrchestrator
    {
        public IEnumerable<CategoryViewModel> GetCategories(string id, out string title);
        public IEnumerable<BlogPostViewModel> GetBlogPosts(string categoryId);
    }
}
