﻿namespace Blog.Features.BlogPost;

public class BlogPostOrchestrator(IBlogPostLoader blogPostLoader, IPageLoader pageLoader) : IBlogPostOrchestrator
{
    public async Task<IEnumerable<BlogPostViewModel>> GetBlogPosts(string id, bool preview)
    {
        if (string.IsNullOrEmpty(id))
        {
            var blogPosts = await blogPostLoader
                .Get(0);

            return blogPosts
                .Select(content => new BlogPostViewModel(content));
        }

        var blogPostContent = preview
            ? await blogPostLoader
                .GetPreview(id)
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