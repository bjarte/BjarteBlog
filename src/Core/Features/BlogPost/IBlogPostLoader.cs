using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Features.BlogPost.Models;

namespace Core.Features.BlogPost;

public interface IBlogPostLoader
{
    Task<BlogPostContent> GetBlogPost(string slug);
    Task<BlogPostContent> GetBlogPostPreview(string id);
    Task<IEnumerable<BlogPostContent>> GetBlogPosts();
    Task<IEnumerable<BlogPostContent>> GetBlogPostsWithCategory(string categorySlug);
}