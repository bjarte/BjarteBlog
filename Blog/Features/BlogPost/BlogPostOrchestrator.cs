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
                    .Get(0)
                    .Result
                    .Select(_ => new BlogPostViewModel(_));
            }

            var blogPostContent = preview
                ? _blogPostLoader
                    .GetPreview(id)
                    .Result
                : _blogPostLoader
                    .Get(id)
                    .Result;

            title = blogPostContent?.Title ?? id;

            return blogPostContent == null
                ? Enumerable.Empty<BlogPostViewModel>()
                : new BlogPostViewModel[] { new(blogPostContent, true) };
        }
    }
}
