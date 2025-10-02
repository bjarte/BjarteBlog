namespace Blog.Features.Category;

public class CategoryLoader(
    IContentfulClient contentDeliveryClient,
    IMemoryCache cache
) : ICategoryLoader
{
    private readonly string _orderAlphabetically = SortOrderBuilder<CategoryContent>
        .New(content => content.Title)
        .Build();

    public async Task<CategoryContent> Get(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var cacheKey = $"contentful_category_{slug}";
        if (cache.TryGetValue(cacheKey, out CategoryContent cachedCategory))
        {
            return cachedCategory;
        }

        var query = new QueryBuilder<CategoryContent>()
            .ContentTypeIs(ContentTypes.Category)
            .FieldEquals(content => content.Slug, slug);

        try
        {
            var category = (await contentDeliveryClient
                    .GetEntries(query))?
                .FirstOrDefault();

            if (category == null)
            {
                return null;
            }

            cache.Set(cacheKey, category);

            return category;
        }
        catch (ContentfulException)
        {
            return null;
        }
    }

    public async Task<IEnumerable<CategoryContent>> Get()
    {
        const string cacheKey = "contentful_all_categories";
        if (cache.TryGetValue(cacheKey, out IEnumerable<CategoryContent> cachedCategories))
        {
            return cachedCategories;
        }

        var query = new QueryBuilder<CategoryContent>()
            .ContentTypeIs("category")
            .Include(1)
            .OrderBy(_orderAlphabetically);

        try
        {
            var categories = await contentDeliveryClient
                .GetEntries(query);

            if (categories == null)
            {
                return [];
            }

            cache.Set(cacheKey, categories);

            return categories;
        }
        catch (ContentfulException)
        {
            return [];
        }
    }
}