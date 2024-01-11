namespace Blog.Features.Navigation.Models;

public class NavigationViewModel
{
    public NavigationViewModel(List<LinkViewModel> links)
    {
        if (links == null)
        {
            Links = [];
            return;
        }

        Links = links;
    }

    public List<LinkViewModel> Links { get; set; }
}