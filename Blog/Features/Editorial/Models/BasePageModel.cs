using PageModel = Microsoft.AspNetCore.Mvc.RazorPages.PageModel;

namespace Blog.Features.Editorial.Models;

public class BasePageModel : PageModel
{
    public string Title { get; set; }

    public NavigationViewModel Navigation { get; set; }
}
