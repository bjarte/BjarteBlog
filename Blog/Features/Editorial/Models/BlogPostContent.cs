namespace Blog.Features.Editorial.Models;

public class BlogPostContent : EditorialContent
{
    public Asset MainImage { get; set; }
    public string Intro { get; set; }
    public List<CategoryContent> Categories { get; set; }
    public bool IncludeInSearchAndNavigation { get; set; }
    public string TypeformFormId { get; set; }
    public DateTime? PublishedAt { get; set; }
}