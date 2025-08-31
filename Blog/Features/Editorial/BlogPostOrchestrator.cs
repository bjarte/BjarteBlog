namespace Blog.Features.Editorial;

public class BlogPostOrchestrator(
    IBlogPostLoader blogPostLoader,
    IPageLoader pageLoader,
    IPreviewLoader previewLoader
) : IBlogPostOrchestrator
{
    public async Task<IEnumerable<BlogPostViewModel>> GetBlogPosts(string id, bool preview)
    {
        if (string.IsNullOrEmpty(id))
        {
            var blogPosts = await blogPostLoader
                .Get();

            return blogPosts
                .Select(content => new BlogPostViewModel(content));
        }

        var blogPostContent = preview
            ? await previewLoader
                .GetPreview<BlogPostContent>(id)
            : await blogPostLoader
                .Get(id);

        if (blogPostContent == null)
        {
            return [];
        }

        var blogPost = new BlogPostViewModel(blogPostContent, true);

        var pageContent = pageLoader.Get("about-me").Result;
        if (pageContent != null)
        {
            blogPost.Author = new PageViewModel(pageContent);
        }

        return [blogPost];
    }
}