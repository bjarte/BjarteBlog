using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
using Core.Features.AppSettings;
using Core.Features.Navigation.Models;

namespace Core.Features.Navigation;

public class NavigationLoader : INavigationLoader
{
    private readonly IAppSettingsService _appSettingsService;
    private readonly IContentfulClient _contentDeliveryClient;

    public NavigationLoader(
        IAppSettingsService appSettingsService, 
        IContentfulClient contentDeliveryClient
    )
    {
        _appSettingsService = appSettingsService;
        _contentDeliveryClient = contentDeliveryClient;
    }

    public async Task<NavigationContent> GetNavigation()
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

        return navigations.FirstOrDefault();
    }
}