using Contentful.Core.Models;

namespace Blog.Features.Renderers;

public interface IHasBody
{
    public Document Body { get; set; }
    public string BodyString { get; set; }
}