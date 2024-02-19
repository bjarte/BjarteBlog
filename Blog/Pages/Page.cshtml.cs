namespace Blog.Pages;

public class PageModel(
    IPageLoader pageLoader,
    INavigationOrchestrator navigationOrchestrator)
    : BasePageModel
{
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

        Navigation = navigationOrchestrator.Get();

        var pageContent = preview
            ? pageLoader
                .GetPreview(id)
                .Result
            : pageLoader
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
