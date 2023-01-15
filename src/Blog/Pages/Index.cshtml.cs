using System.Text.RegularExpressions;
using Core.Features.BlogPost;
using Core.Features.BlogPost.Models;
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

    public IActionResult OnGet(bool disableCache = false, string year = null, string month = null, string slug = null)
    {
        // Handle old blog urls on the format
        // /2019/03/blog-post-slug
        if (YearRegex().IsMatch(year ?? string.Empty)
            && MonthRegex().IsMatch(month ?? string.Empty))
        {
            return RedirectToPagePermanent("BlogPost", new { id = slug });
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
    private static partial Regex YearRegex();
    [GeneratedRegex("^\\d\\d$")]
    private static partial Regex MonthRegex();
}
