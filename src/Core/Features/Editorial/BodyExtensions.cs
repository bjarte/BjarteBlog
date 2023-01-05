using System.Linq;
using Contentful.Core.Models;

namespace Core.Features.Editorial;

public static class BodyExtensions
{
    public static IHasBody BodyToHtml(this IHasBody content)
    {
        if (content?.Body?.Content?.Any() == true)
        {
            content.BodyString = new HtmlRenderer().ToHtml(content.Body).Result;
        }

        return content;
    }
}