using System.Threading.Tasks;
using Blog.Features.Contentful;
using Blog.Features.Navigation.Models;
using Contentful.Core;
using Contentful.Core.Search;
using Microsoft.Extensions.Options;

namespace Blog.Features.Navigation;

public class LinkLoader(
    IOptions<ContentfulConfig> contentfulConfig,
    IContentfulClient contentDeliveryClient
) : ILinkLoader
{
    public async Task<IEnumerable<LinkContent>> Get()
    {
        var navigationSlug = contentfulConfig.Value.Navigation;

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