using Blog.Features.BlogPost.Models;
using Blog.Features.Category;
using Blog.Features.Category.Models;
using Blog.Features.Navigation;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Pages;

public class CategoryModel : BasePageModel
{
    private readonly ICategoryOrchestrator _orchestrator;
    private readonly INavigationOrchestrator _navigationOrchestrator;

    public CategoryModel(
        ICategoryOrchestrator orchestrator,
        INavigationOrchestrator navigationOrchestrator
    )
    {
        _orchestrator = orchestrator;
        _navigationOrchestrator = navigationOrchestrator;
    }

    public string Id { get; set; }
    public IEnumerable<CategoryViewModel> Categories { get; set; }
    public IEnumerable<BlogPostViewModel> BlogPosts { get; set; }

    public IActionResult OnGet(string id, bool disableCache = false)
    {
        Id = id;

        if (!disableCache)
        {
            HttpContext.EnableOutputCaching(
                TimeSpan.FromMinutes(60),
                varyByParam: $"{nameof(id)},{nameof(disableCache)}");
        }

        Navigation = _navigationOrchestrator.Get();

        Categories = _orchestrator.GetCategories(id, out var title);
        Title = title;

        BlogPosts = _orchestrator.GetBlogPosts(id);

        return Page();
    }
}
