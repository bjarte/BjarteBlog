namespace Blog.Features.Category;

public interface ICategoryLoader
{
    Task<CategoryContent> Get(string slug);
    Task<IEnumerable<CategoryContent>> Get();
}
