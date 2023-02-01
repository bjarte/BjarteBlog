using Blog.Features.Navigation;
using Blog.Features.Navigation.Models;
using Blog.Features.Page;
using Blog.Features.Page.Models;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Pages;

public class PageModel : BasePageModel
{
    private readonly IPageLoader _pageLoader;
    private readonly INavigationLoader _navigationLoader;

    public PageModel(
        IPageLoader pageLoader,
        INavigationLoader navigationLoader
    )
    {
        _pageLoader = pageLoader;
        _navigationLoader = navigationLoader;
    }

    public string Id { get; set; }
    public PageViewModel CurrentPage { get; set; }

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

        var pageContent = preview
            ? _pageLoader
                .GetPagePreview(id)
                .Result
            : _pageLoader
                .GetPage(id)
                .Result;

        if (pageContent != null)
        {
            CurrentPage = new PageViewModel(pageContent, true);
        }

        Title = CurrentPage?.Title ?? string.Empty;

        return Page();
    }
}
