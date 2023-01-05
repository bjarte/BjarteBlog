using Core.Features.BlogPost;
using Core.Features.BlogPost.Models;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Pages;

public class IndexModel : BasePageModel
{
    private readonly IBlogPostLoader _blogPostLoader;
    private readonly INavigationLoader _navigationLoader;

    public IndexModel(
        IBlogPostLoader blogPostLoader,
        INavigationLoader navigationLoader
    )
    {
        _blogPostLoader = blogPostLoader;
        _navigationLoader = navigationLoader;
    }

    public IEnumerable<BlogPostViewModel> BlogPosts { get; set; }

    public IActionResult OnGet(bool disableCache = false)
    {
        if (!disableCache)
        {
            HttpContext.EnableOutputCaching(
                TimeSpan.FromMinutes(60),
                varyByParam: nameof(disableCache));
        }

        var navigationContent = _navigationLoader
            .GetNavigation()
            .Result;

        if (navigationContent != null)
        {
            Navigation = new NavigationViewModel(navigationContent);
        }

        BlogPosts = _blogPostLoader
            .GetBlogPosts()
            .Result
            .Select(_ => new BlogPostViewModel(_));

        return Page();
    }
}
