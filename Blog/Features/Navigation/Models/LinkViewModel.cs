namespace Blog.Features.Navigation.Models
{
    public class LinkViewModel
    {
        public LinkViewModel(LinkContent content)
        {
            if (content == null)
            {
                Title = "Link";
                Url = "/";
                return;
            }

            Title = content.Title;
            Url = content.ExternalLink;
        }

        public string Title { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
    }
}
