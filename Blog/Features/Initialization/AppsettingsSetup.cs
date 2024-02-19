using Blog.Features.Caching;

namespace Blog.Features.Initialization;

public static class AppsettingsSetup
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