namespace Core.Features.Navigation.Models
{
    public class LinkViewModel
    {
        public LinkViewModel(LinkContent content)
        {
            if (content == null)
            {
                Title = "Link";
                Slug = "link";
                Url = "/";
                return;
            }

            Title = content.Title;
            Slug = content.Slug;
            Url = content.ExternalLink;
        }

        public string Title { get; set; }
        public string Slug { get; set; }
        public string Url { get; set; }
    }
}
