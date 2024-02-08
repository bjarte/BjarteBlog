using Blog.Features.BlogPost;
using Blog.Features.Category;
using Blog.Features.Navigation;
using Blog.Features.Page;
using Blog.Features.Renderers;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Features.Initialization
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IBlogPostLoader, BlogPostLoader>();
            services.AddSingleton<IBlogPostOrchestrator, BlogPostOrchestrator>();
            services.AddSingleton<ICategoryLoader, CategoryLoader>();
            services.AddSingleton<ICategoryOrchestrator, CategoryOrchestrator>();
            services.AddSingleton<ILinkLoader, LinkLoader>();
            services.AddSingleton<INavigationOrchestrator, NavigationOrchestrator>();
            services.AddSingleton<IPageLoader, PageLoader>();

            services.AddSingleton<ICodeBlockContentRenderer, CodeBlockContentRenderer>();
            services.AddSingleton<IRichTextRenderer, RichTextRenderer>();

            return services;
        }
    }
}
