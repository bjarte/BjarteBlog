using Blog.Features.BlogPost.Models;

namespace Blog.Features.BlogPost
{
    public class BlogPostOrchestrator : IBlogPostOrchestrator
    {
        private readonly IBlogPostLoader _blogPostLoader;

        public BlogPostOrchestrator(IBlogPostLoader blogPostLoader)
        {
            _blogPostLoader = blogPostLoader;
        }

        public IEnumerable<BlogPostViewModel> GetBlogPosts(string id, bool preview, out string title)
        {
            if (string.IsNullOrEmpty(id))
            {
                title = "Blog posts";

                return _blogPostLoader
                    .GetBlogPosts()
                    .Result
                    .Select(_ => new BlogPostViewModel(_));
            }

            var blogPostContent = preview
                ? _blogPostLoader
                    .GetBlogPostPreview(id)
                    .Result
                : _blogPostLoader
                    .GetBlogPost(id)
                    .Result;

            title = blogPostContent?.Title ?? id;

            return blogPostContent == null
                ? Enumerable.Empty<BlogPostViewModel>()
                : new BlogPostViewModel[] { new(blogPostContent, true) };
        }
    }
}
