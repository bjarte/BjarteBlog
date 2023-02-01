using System.Threading.Tasks;
using Blog.Features.BlogPost.Models;

namespace Blog.Features.BlogPost;

public interface IBlogPostLoader
{
    Task<BlogPostContent> GetBlogPost(string slug);
    Task<BlogPostContent> GetBlogPostPreview(string id);
    Task<IEnumerable<BlogPostContent>> GetBlogPosts();
    Task<IEnumerable<BlogPostContent>> GetBlogPostsWithCategory(string categorySlug);
}