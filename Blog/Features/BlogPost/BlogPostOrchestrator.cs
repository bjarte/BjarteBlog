using Blog.Features.BlogPost.Models;
using Blog.Features.Page;
using Blog.Features.Page.Models;

namespace Blog.Features.BlogPost;

public class BlogPostOrchestrator(IBlogPostLoader blogPostLoader, IPageLoader pageLoader) : IBlogPostOrchestrator
{
    public IEnumerable<BlogPostViewModel> GetBlogPosts(string id, bool preview, out string title)
    {
        if (string.IsNullOrEmpty(id))
        {
            title = "Blog posts";

            return blogPostLoader
                .Get(0)
                .Result
                .Select(_ => new BlogPostViewModel(_));
        }

        var blogPostContent = preview
            ? blogPostLoader
                .GetPreview(id)
                .Result
            : blogPostLoader
                .Get(id)
                .Result;

        title = blogPostContent?.Title ?? id;

        if (blogPostContent == null)
        {
            return Enumerable.Empty<BlogPostViewModel>();
        }

        var blogPost = new BlogPostViewModel(blogPostContent, true);

        var pageContent = pageLoader.Get("about-me").Result;
        if (pageContent != null)
        {
            blogPost.Author = new PageViewModel(pageContent);
        }

        return new[] { blogPost };
    }
}