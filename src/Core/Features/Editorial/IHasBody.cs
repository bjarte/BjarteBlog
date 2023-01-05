using Contentful.Core.Models;

namespace Core.Features.Editorial;

public interface IHasBody
{
    public Document Body { get; set; }
    public string BodyString { get; set; }
}