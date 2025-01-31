namespace Blog.Pages;

public class BlogPostModel(IBlogPostOrchestrator blogPostOrchestrator, INavigationOrchestrator navigationOrchestrator) : BasePageModel
{
    public string Id { get; set; }
    public IEnumerable<BlogPostViewModel> BlogPosts { get; set; }

    public async Task<IActionResult> OnGet(string id, bool preview = false, bool disableCache = false)
    {
        Id = id;

        if (!preview && !disableCache)
        {
            HttpContext.EnableOutputCaching(
                TimeSpan.FromMinutes(60),
                varyByParam: $"{nameof(id)},{nameof(preview)},{nameof(disableCache)}");
        }

        Navigation = await navigationOrchestrator.Get();

        BlogPosts = await blogPostOrchestrator.GetBlogPosts(id, preview);

        Title = !string.IsNullOrEmpty(id)
            ? BlogPosts.FirstOrDefault()?.Title ?? id
            : "Blog posts";

        return Page();
    }
}
