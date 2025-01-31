namespace Blog.Features.Navigation.Models;

public class NavigationContent : ContentfulContent
{
    public string Slug { get; set; }

    public List<LinkContent> Links { get; set; }
}