namespace Blog.Features.Navigation;

public class NavigationOrchestrator(
    IBlogPostLoader blogPostLoader,
    ILinkLoader linkLoader,
    IPageLoader pageLoader
) : INavigationOrchestrator
{
    public NavigationViewModel Get()
    {
        var links = linkLoader
            .Get()?
            .Result;

        var linkList = new List<LinkViewModel>();

        foreach (var link in links ?? [])
        {
            var linkViewModel = new LinkViewModel(link);

            var linkedContentId = link?.InternalLink?.Sys?.Id;
            if (!string.IsNullOrEmpty(linkedContentId))
            {
                var pageSlug = pageLoader
                    .GetSlug(linkedContentId)?
                    .Result;
                if (!string.IsNullOrEmpty(pageSlug))
                {
                    linkViewModel.Path = "/page/" + pageSlug;
                }

                var blogSlug = blogPostLoader
                    .GetSlug(linkedContentId)?
                    .Result;
                if (!string.IsNullOrEmpty(blogSlug))
                {
                    linkViewModel.Path = "/blogpost/" + blogSlug;
                }
            }

            linkList.Add(linkViewModel);
        }

        return new NavigationViewModel(linkList);
    }
}