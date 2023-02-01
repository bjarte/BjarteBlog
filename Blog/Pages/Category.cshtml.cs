using Blog.Features.BlogPost.Models;
using Blog.Features.Category;
using Blog.Features.Category.Models;
using Blog.Features.Navigation;
using Blog.Features.Navigation.Models;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Pages;

public class CategoryModel : BasePageModel
{
    private readonly ICategoryOrchestrator _orchestrator;
    private readonly INavigationLoader _navigationLoader;

    public CategoryModel(
        ICategoryOrchestrator orchestrator,
        INavigationLoader navigationLoader
    )
    {
        _orchestrator = orchestrator;
        _navigationLoader = navigationLoader;
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

        var navigationContent = _navigationLoader
            .GetNavigation()
            .Result;

        if (navigationContent != null)
        {
            Navigation = new NavigationViewModel(navigationContent);
        }

        Categories = _orchestrator.GetCategories(id, out var title);
        Title = title;

        BlogPosts = _orchestrator.GetBlogPosts(id);

        return Page();
    }
}
