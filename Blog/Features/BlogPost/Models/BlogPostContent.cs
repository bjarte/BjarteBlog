namespace Blog.Features.BlogPost.Models;

public class BlogPostContent : ContentfulContent, IHasBody
{
    public string Title { get; set; }
    public string Slug { get; set; }

    public Asset MainImage { get; set; }
    public string Intro { get; set; }
    public Document Body { get; set; }
    public string BodyString { get; set; }

    public List<CategoryContent> Categories { get; set; }

    public bool IncludeInSearchAndNavigation { get; set; }

    public string TypeformFormId { get; set; }

    public DateTime? PublishedAt { get; set; }
}