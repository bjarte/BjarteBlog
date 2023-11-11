using System.Threading.Tasks;
using Blog.Features.Category.Models;

namespace Blog.Features.Category;

public interface ICategoryLoader
{
    Task<CategoryContent> Get(string slug);
    Task<IEnumerable<CategoryContent>> Get();
}
