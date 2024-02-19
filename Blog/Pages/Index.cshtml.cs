namespace Blog.Pages;

public partial class IndexModel(IBlogPostLoader blogPostLoader, INavigationOrchestrator navigationOrchestrator, IPageLoader pageLoader) : BasePageModel
{
    public IEnumerable<BlogPostViewModel> BlogPosts { get; set; }
    public PageViewModel Author { get; set; }

    public IActionResult OnGet(bool disableCache = false, string param1 = null, string param2 = null, string param3 = null, string param4 = null)
    {
        // Handle old blog urls on the format
        // /2020/12/31/blog-post-slug
        if (FourDigitRegex().IsMatch(param1 ?? string.Empty)
            && TwoDigitRegex().IsMatch(param2 ?? string.Empty)
            && TwoDigitRegex().IsMatch(param3 ?? string.Empty))
        {
            return RedirectToPagePermanent("BlogPost", new
            {
                id = param4
            });
        }

        // Handle old blog urls on the format
        // /2020/12/blog-post-slug
        if (FourDigitRegex().IsMatch(param1 ?? string.Empty)
            && TwoDigitRegex().IsMatch(param2 ?? string.Empty))
        {
            return RedirectToPagePermanent("BlogPost", new
            {
                id = param3
            });
        }

        if (!disableCache)
        {
            HttpContext.EnableOutputCaching(
                TimeSpan.FromMinutes(60),
                varyByParam: nameof(disableCache));
        }

        Navigation = navigationOrchestrator.Get();

        BlogPosts = blogPostLoader
            .Get(10)
            .Result
            .Select(_ => new BlogPostViewModel(_));

        Author = new PageViewModel(pageLoader.Get("about-me").Result);

        return Page();
    }

    [GeneratedRegex("^\\d\\d\\d\\d$")]
    private static partial Regex FourDigitRegex();
    [GeneratedRegex("^\\d\\d$")]
    private static partial Regex TwoDigitRegex();
}
