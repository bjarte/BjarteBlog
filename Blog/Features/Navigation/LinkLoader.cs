using System.Threading.Tasks;
using Blog.Features.AppSettings;
using Blog.Features.Navigation.Models;
using Contentful.Core;
using Contentful.Core.Search;

namespace Blog.Features.Navigation;

public class LinkLoader(IAppSettingsService appSettingsService, IContentfulClient contentDeliveryClient) : ILinkLoader
{
    public async Task<IEnumerable<LinkContent>> Get()
    {
        var navigationSlug = appSettingsService.GetContentfulNavigation();

        if (string.IsNullOrWhiteSpace(navigationSlug))
        {
            return null;
        }

        var query = new QueryBuilder<NavigationContent>()
            .ContentTypeIs("navigation")
            .FieldEquals(_ => _.Slug, navigationSlug)
            .Include(2);

        var navigations = await contentDeliveryClient
            .GetEntries(query);

        return navigations.FirstOrDefault()?.Links;
    }
}