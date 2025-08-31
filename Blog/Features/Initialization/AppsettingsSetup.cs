namespace Blog.Features.Initialization;

public static class AppsettingsSetup
{
    public static IServiceCollection AddAppsettings(this IServiceCollection services)
    {
        services.AddOptions<ContentfulConfig>()
            .BindConfiguration("ContentfulOptions");

        return services;
    }
}