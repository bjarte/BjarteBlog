using System.Threading.Tasks;
using Blog.Features.AppSettings;
using Blog.Features.Navigation.Models;
using Contentful.Core;
using Contentful.Core.Search;

namespace Blog.Features.Navigation;

public class LinkLoader : ILinkLoader
{
    private readonly IAppSettingsService _appSettingsService;
    private readonly IContentfulClient _contentDeliveryClient;

    public LinkLoader(
        IAppSettingsService appSettingsService,
        IContentfulClient contentDeliveryClient
    )
    {
        _appSettingsService = appSettingsService;
        _contentDeliveryClient = contentDeliveryClient;
    }

    public async Task<IEnumerable<LinkContent>> Get()
    {
        var navigationSlug = _appSettingsService.GetContentfulNavigation();

        if (string.IsNullOrWhiteSpace(navigationSlug))
        {
            return null;
        }

        var query = new QueryBuilder<NavigationContent>()
            .ContentTypeIs("navigation")
            .FieldEquals(_ => _.Slug, navigationSlug)
            .Include(2);

        var navigations = await _contentDeliveryClient
            .GetEntries(query);

        return navigations.FirstOrDefault()?.Links;
    }
}