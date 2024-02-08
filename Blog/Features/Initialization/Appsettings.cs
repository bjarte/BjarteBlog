using Blog.Features.Caching;
using Blog.Features.Contentful;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Features.Initialization;

public static class Appsettings
{
    public static IServiceCollection AddAppsettings(this IServiceCollection services)
    {
        services.AddOptions<ContentfulConfig>()
            .BindConfiguration("ContentfulOptions");

        services.AddOptions<OutputCacheConfig>()
            .BindConfiguration("OutputCacheOptions");

        return services;
    }
}