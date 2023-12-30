using Blog.Features.BlogPost.Models;
using Blog.Features.Category;
using Blog.Features.Category.Models;
using Blog.Features.Navigation;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Pages;

public class CategoryModel(
    ICategoryOrchestrator orchestrator,
    INavigationOrchestrator navigationOrchestrator)
    : BasePageModel
{
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

        Navigation = navigationOrchestrator.Get();

        Categories = orchestrator.GetCategories(id, out var title);
        Title = title;

        BlogPosts = orchestrator.GetBlogPosts(id);

        return Page();
    }
}
