namespace Blog.Features.Navigation.Models
{
    public class NavigationViewModel
    {
        public NavigationViewModel(NavigationContent content)
        {
            if (content == null)
            {
                Title = "Mangler tittel";
                Links = new List<LinkViewModel>();
                return;
            }

            Title = content.Title;
            Links = content
                .Links
                .Select(_ => new LinkViewModel(_))
                .ToList();
        }

        public string Title { get; set; }
        public List<LinkViewModel> Links { get; set; }
    }
}
