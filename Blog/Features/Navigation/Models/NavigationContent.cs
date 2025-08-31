namespace Blog.Features.Navigation.Models;

public class NavigationContent : IContent
{
    public string Slug { get; set; }
    public List<LinkContent> Links { get; set; }
}