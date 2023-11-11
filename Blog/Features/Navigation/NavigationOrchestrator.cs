using Blog.Features.BlogPost;
using Blog.Features.Navigation.Models;
using Blog.Features.Page;

namespace Blog.Features.Navigation
{
    public class NavigationOrchestrator : INavigationOrchestrator
    {
        private readonly IBlogPostLoader _blogPostLoader;
        private readonly ILinkLoader _linkLoader;
        private readonly IPageLoader _pageLoader;

        public NavigationOrchestrator(
            IBlogPostLoader blogPostLoader,
            ILinkLoader linkLoader,
            IPageLoader pageLoader
        )
        {
            _blogPostLoader = blogPostLoader;
            _linkLoader = linkLoader;
            _pageLoader = pageLoader;
        }

        public NavigationViewModel Get()
        {
            var links = _linkLoader
                .Get()?
                .Result;

            var linkList = new List<LinkViewModel>();

            foreach (var link in links ?? Enumerable.Empty<LinkContent>())
            {
                var linkViewModel = new LinkViewModel(link);

                var linkedContentId = link?.InternalLink?.Sys?.Id;
                if (!string.IsNullOrEmpty(linkedContentId))
                {
                    var pageSlug = _pageLoader
                        .GetSlug(linkedContentId)?
                        .Result;
                    if (!string.IsNullOrEmpty(pageSlug))
                    {
                        linkViewModel.Path = "/page/" + pageSlug;
                    }

                    var blogSlug = _blogPostLoader
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
}
