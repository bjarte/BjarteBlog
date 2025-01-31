using Contentful.Core.Errors;

namespace Blog.Features.Category;

public class CategoryLoader : ICategoryLoader
{
    private readonly IContentfulClient _contentDeliveryClient;
    private readonly string _orderAlphabetically;

    public CategoryLoader(IContentfulClient contentDeliveryClient)
    {
        _contentDeliveryClient = contentDeliveryClient;

        _orderAlphabetically = SortOrderBuilder<CategoryContent>
            .New(content => content.Title)
            .Build();
    }

    public async Task<CategoryContent> Get(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var query = new QueryBuilder<CategoryContent>()
            .ContentTypeIs("category")
            .FieldEquals(content => content.Slug, slug);

        try
        {
            var categories = await _contentDeliveryClient
                .GetEntries(query);

            return categories.FirstOrDefault();
        }
        catch (ContentfulException)
        {
            return null;
        }
    }

    public async Task<IEnumerable<CategoryContent>> Get()
    {
        var query = new QueryBuilder<CategoryContent>()
            .ContentTypeIs("category")
            .Include(1)
            .OrderBy(_orderAlphabetically);

        try
        {
            return await _contentDeliveryClient
                .GetEntries(query); ;
        }
        catch (ContentfulException)
        {
            return [];
        }
    }
}