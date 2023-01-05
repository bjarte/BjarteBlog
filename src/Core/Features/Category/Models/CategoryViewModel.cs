namespace Core.Features.Category.Models;

public class CategoryViewModel
{
    public CategoryViewModel(CategoryContent content)
    {
        if (content == null)
        {
            Title = "Unknown Category";
            Slug = "unknown";
            return;
        }

        Title = content.Title;
        Slug = content.Slug;
    }

    public string Title { get; set; }
    public string Slug { get; set; }
}