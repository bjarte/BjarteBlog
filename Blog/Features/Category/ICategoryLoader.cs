using System.Threading.Tasks;
using Blog.Features.Category.Models;

namespace Blog.Features.Category;

public interface ICategoryLoader
{
    Task<CategoryContent> GetCategory(string slug);
    Task<IEnumerable<CategoryContent>> GetCategories();
}
