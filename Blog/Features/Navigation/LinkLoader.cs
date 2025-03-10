using Contentful.Core.Errors;

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
            .FieldEquals(content => content.Slug, navigationSlug)
            .Include(2);

        try
        {
            var navigations = await contentDeliveryClient
                .GetEntries(query);

            return navigations?.FirstOrDefault()?.Links;
        }
        catch (ContentfulException)
        {
            return [];
        }
    }
}