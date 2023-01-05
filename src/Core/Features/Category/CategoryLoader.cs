using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Search;
using Core.Features.Category.Models;

namespace Core.Features.Category;

public class CategoryLoader : ICategoryLoader
{
    private readonly IContentfulClient _contentDeliveryClient;
    private readonly string _orderAlphabetically;

    public CategoryLoader(IContentfulClient contentDeliveryClient)
    {
        _contentDeliveryClient = contentDeliveryClient;

        _orderAlphabetically = SortOrderBuilder<CategoryContent>
            .New(_ => _.Title)
            .Build();
    }

    public async Task<CategoryContent> GetCategory(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var query = new QueryBuilder<CategoryContent>()
            .ContentTypeIs("category")
            .FieldEquals(_ => _.Slug, slug);

        var entries = await _contentDeliveryClient
            .GetEntries(query);

        return entries.FirstOrDefault();
    }

    public async Task<IEnumerable<CategoryContent>> GetCategories()
    {
        var query = new QueryBuilder<CategoryContent>()
            .ContentTypeIs("category")
            .Include(1)
            .OrderBy(_orderAlphabetically);

        return await _contentDeliveryClient
            .GetEntries(query);
    }
}