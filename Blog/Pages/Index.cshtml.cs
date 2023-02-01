using System.Text.RegularExpressions;
using Blog.Features.BlogPost;
using Blog.Features.BlogPost.Models;
using Blog.Features.Navigation;
using Blog.Features.Navigation.Models;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Pages;

public partial class IndexModel : BasePageModel
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

    public IActionResult OnGet(bool disableCache = false, string param1 = null, string param2 = null, string param3 = null, string param4 = null)
    {
        // Handle old blog urls on the format
        // /2020/12/31/blog-post-slug
        if (FourDigitRegex().IsMatch(param1 ?? string.Empty)
            && TwoDigitRegex().IsMatch(param2 ?? string.Empty)
            && TwoDigitRegex().IsMatch(param3 ?? string.Empty))
        {
            return RedirectToPagePermanent("BlogPost", new { id = param4 });
        }

        // Handle old blog urls on the format
        // /2020/12/blog-post-slug
        if (FourDigitRegex().IsMatch(param1 ?? string.Empty)
            && TwoDigitRegex().IsMatch(param2 ?? string.Empty))
        {
            return RedirectToPagePermanent("BlogPost", new { id = param3 });
        }

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

    [GeneratedRegex("^\\d\\d\\d\\d$")]
    private static partial Regex FourDigitRegex();
    [GeneratedRegex("^\\d\\d$")]
    private static partial Regex TwoDigitRegex();
}
