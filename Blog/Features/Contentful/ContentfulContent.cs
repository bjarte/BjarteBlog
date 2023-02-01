using Contentful.Core.Models;

namespace Blog.Features.Contentful;

public class ContentfulContent : IContent
{
    public SystemProperties Sys { get; set; }
}