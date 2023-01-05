using Core.Features.BlogPost;
using Core.Features.BlogPost.Models;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Pages;

public class BlogPostModel : BasePageModel
{
    private readonly IBlogPostOrchestrator _blogPostOrchestrator;
    private readonly INavigationLoader _navigationLoader;

    public BlogPostModel(
        IBlogPostOrchestrator blogPostOrchestrator, 
        INavigationLoader navigationLoader
    )
    {
        _blogPostOrchestrator = blogPostOrchestrator;
        _navigationLoader = navigationLoader;
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

        var navigationContent = _navigationLoader
            .GetNavigation()
            .Result;

        if (navigationContent != null)
        {
            Navigation = new NavigationViewModel(navigationContent);
        }

        BlogPosts = _blogPostOrchestrator.GetBlogPosts(id, preview, out var title);
        Title = title;

        return Page();
    }
}
