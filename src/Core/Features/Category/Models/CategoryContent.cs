using Core.Features.Contentful;

namespace Core.Features.Category.Models;

public class CategoryContent : ContentfulContent
{
    public string Title { get; set; }
    public string Slug { get; set; }
}