using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Features.Category.Models;

namespace Core.Features.Category;

public interface ICategoryLoader
{
    Task<CategoryContent> GetCategory(string slug);
    Task<IEnumerable<CategoryContent>> GetCategories();
}
