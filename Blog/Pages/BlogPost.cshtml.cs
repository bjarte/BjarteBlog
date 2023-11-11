using Blog.Features.BlogPost;
using Blog.Features.BlogPost.Models;
using Blog.Features.Navigation;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Pages;

public class BlogPostModel : BasePageModel
{
    private readonly IBlogPostOrchestrator _blogPostOrchestrator;
    private readonly INavigationOrchestrator _navigationOrchestrator;

    public BlogPostModel(
        IBlogPostOrchestrator blogPostOrchestrator,
        INavigationOrchestrator navigationOrchestrator
    )
    {
        _blogPostOrchestrator = blogPostOrchestrator;
        _navigationOrchestrator = navigationOrchestrator;
    }

    public string Id { get; set; }
    public IEnumerable<BlogPostViewModel> BlogPosts { get; set; }

    public IActionResult OnGet(string id, bool preview = false, bool disableCache = false)
    {
        Id = id;

        if (!preview && !disableCache)
        {
            HttpContext.EnableOutputCaching(
                TimeSpan.FromMinutes(60),
                varyByParam: $"{nameof(id)},{nameof(preview)},{nameof(disableCache)}");
        }

        Navigation = _navigationOrchestrator.Get();

        BlogPosts = _blogPostOrchestrator.GetBlogPosts(id, preview, out var title);
        Title = title;

        return Page();
    }
}
