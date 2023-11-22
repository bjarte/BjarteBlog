using Blog.Features.BlogPost.Models;

namespace Blog.Features.BlogPost;

public class BlogPostOrchestrator(IBlogPostLoader blogPostLoader) : IBlogPostOrchestrator
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

        return blogPostContent == null
            ? Enumerable.Empty<BlogPostViewModel>()
            : new BlogPostViewModel[] { new(blogPostContent, true) };
    }
}