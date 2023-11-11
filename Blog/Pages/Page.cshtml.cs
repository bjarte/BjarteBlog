using Blog.Features.Navigation;
using Blog.Features.Page;
using Blog.Features.Page.Models;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Pages;

public class PageModel : BasePageModel
{
    private readonly IPageLoader _pageLoader;
    private readonly INavigationOrchestrator _navigationOrchestrator;

    public PageModel(
        IPageLoader pageLoader,
        INavigationOrchestrator navigationOrchestrator
    )
    {
        _pageLoader = pageLoader;
        _navigationOrchestrator = navigationOrchestrator;
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

        Navigation = _navigationOrchestrator.Get();

        var pageContent = preview
            ? _pageLoader
                .GetPreview(id)
                .Result
            : _pageLoader
                .Get(id)
                .Result;

        if (pageContent != null)
        {
            CurrentPage = new PageViewModel(pageContent, true);
        }

        Title = CurrentPage?.Title ?? string.Empty;

        return Page();
    }
}
