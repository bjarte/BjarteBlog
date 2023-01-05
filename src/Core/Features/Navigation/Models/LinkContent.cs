using Contentful.Core.Models.Management;
using Core.Features.Contentful;

namespace Core.Features.Navigation.Models;

public class LinkContent : ContentfulContent
{
    public string Title { get; set; }
    public string Slug { get; set; }

    public Reference InternalLink { get; set; }
    public string ExternalLink { get; set; }
}