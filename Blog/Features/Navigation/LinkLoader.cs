namespace Blog.Features.Navigation;

public class LinkLoader(
    IOptions<ContentfulConfig> contentfulConfig,
    IContentfulClient contentDeliveryClient,
    IMemoryCache cache
) : ILinkLoader
{
    public async Task<IEnumerable<LinkContent>> Get()
    {
        var navigationSlug = contentfulConfig.Value.Navigation;

        if (string.IsNullOrWhiteSpace(navigationSlug))
        {
            return null;
        }

        var cacheKey = $"contentful_navigation_links_{navigationSlug}";
        if (cache.TryGetValue(cacheKey, out IEnumerable<LinkContent> cachedLinks))
        {
            return cachedLinks;
        }

        var query = new QueryBuilder<NavigationContent>()
            .ContentTypeIs(ContentTypes.Navigation)
            .FieldEquals(content => content.Slug, navigationSlug)
            .Include(2);

        try
        {
            var links = (await contentDeliveryClient
                    .GetEntries(query))?
                .FirstOrDefault()?
                .Links;

            if (links == null)
            {
                return [];
            }

            cache.Set(cacheKey, links, MemoryCacheConstants.SlidingExpiration1Day);

            return links;
        }
        catch (ContentfulException)
        {
            return [];
        }
    }
}