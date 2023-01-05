using System.Collections.Generic;
using Core.Features.Contentful;

namespace Core.Features.Navigation.Models;

public class NavigationContent : ContentfulContent
{
    public string Title { get; set; }
    public string Slug { get; set; }

    public List<LinkContent> Links { get; set; }
}