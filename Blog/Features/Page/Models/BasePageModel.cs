using PageModel = Microsoft.AspNetCore.Mvc.RazorPages.PageModel;

namespace Blog.Features.Page.Models;

// Base Razor Page model
public class BasePageModel : PageModel
{
    public string Title { get; set; }

    public NavigationViewModel Navigation { get; set; }
}
