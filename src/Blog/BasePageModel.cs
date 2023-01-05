using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog;

// Base Razor Page model
public class BasePageModel : PageModel
{
    public string Title { get; set; }

    public NavigationViewModel Navigation { get; set; }
}
