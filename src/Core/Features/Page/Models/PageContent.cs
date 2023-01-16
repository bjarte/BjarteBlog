using Contentful.Core.Models;
using Core.Features.Contentful;
using Core.Features.Renderers;

namespace Core.Features.Page.Models;

public class PageContent : ContentfulContent, IHasBody
{
    public string Title { get; set; }
    public string Slug { get; set; }

    public bool IncludeInSearchAndNavigation { get; set; }

    public Asset MainImage { get; set; }
    public Document Body { get; set; }
    public string BodyString { get; set; }
}