namespace Blog.Features.Editorial.Models;

public class PageContent : EditorialContent
{
    public bool IncludeInSearchAndNavigation { get; set; }
    public Asset MainImage { get; set; }
}