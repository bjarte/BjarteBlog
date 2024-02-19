namespace Blog.Pages;

public class BlogPostModel(IBlogPostOrchestrator blogPostOrchestrator, INavigationOrchestrator navigationOrchestrator) : BasePageModel
{
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

        Navigation = navigationOrchestrator.Get();

        BlogPosts = blogPostOrchestrator.GetBlogPosts(id, preview, out var title);
        Title = title;

        return Page();
    }
}
