namespace Blog.Pages;

public class PageModel(
    IPageLoader pageLoader,
    IPreviewLoader previewLoader,
    INavigationOrchestrator navigationOrchestrator)
    : BasePageModel
{
    public string Id { get; set; }
    public PageViewModel CurrentPage { get; set; }

    public async Task<IActionResult> OnGet(string id, bool preview = false)
    {
        Id = id;

        Navigation = await navigationOrchestrator.Get();

        var pageContent = preview
            ? previewLoader
                .GetPreview<PageContent>(id)
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
