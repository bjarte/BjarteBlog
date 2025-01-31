namespace Blog.Pages;

public class CategoryModel(
    ICategoryOrchestrator orchestrator,
    INavigationOrchestrator navigationOrchestrator)
    : BasePageModel
{
    public string Id { get; set; }
    public IEnumerable<CategoryViewModel> Categories { get; set; }
    public IEnumerable<BlogPostViewModel> BlogPosts { get; set; }

    public async Task<IActionResult> OnGet(string id, bool disableCache = false)
    {
        Id = id;

        if (!disableCache)
        {
            HttpContext.EnableOutputCaching(
                TimeSpan.FromMinutes(60),
                varyByParam: $"{nameof(id)},{nameof(disableCache)}");
        }

        Navigation = await navigationOrchestrator.Get();

        Categories = await orchestrator.GetCategories(id);

        Title = Categories.Count() == 1
            ? $"Category: {Categories.FirstOrDefault()?.Title ?? id}"
            : "Categories";

        BlogPosts = await orchestrator.GetBlogPosts(id);

        return Page();
    }
}
