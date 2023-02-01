using System.Diagnostics;

namespace Blog.Pages;

[IgnoreAntiforgeryToken]
public class ErrorModel : BasePageModel
{
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public void OnGet()
    {
        Title = "Error";
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
