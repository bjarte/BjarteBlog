using Contentful.Core.Models;

namespace Core.Features.Contentful;

public class ContentfulContent : IContent
{
    public SystemProperties Sys { get; set; }
}