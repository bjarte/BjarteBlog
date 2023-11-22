using System.Net;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace Blog.Features.Renderers;

public class AdvancedTextRenderer : IContentRenderer
{
    public int Order { get; set; } = 10;

    public bool SupportsContent(IContent content)
    {
        return content is Text;
    }

    public Task<string> RenderAsync(IContent content)
    {
        if (content is not Text text
            || string.IsNullOrEmpty(text.Value))
        {
            return Task.FromResult(string.Empty);

        }

        var html = new StringBuilder();

        foreach (var mark in text.Marks ?? new List<Mark>())
        {
            if (mark.Type.Equals("code"))
            {
                html.Append("<pre>");
            }
            html.Append($"<{MarkToHtmlTag(mark)}>");
        }

        var encodedText = WebUtility.HtmlEncode(text.Value);

        html.Append(encodedText);

        foreach (var mark in text.Marks ?? new List<Mark>())
        {
            html.Append($"</{MarkToHtmlTag(mark)}>");
            if (mark.Type.Equals("code"))
            {
                html.Append("</pre>");
            }
        }

        return Task.FromResult(html.ToString());
    }

    private static string MarkToHtmlTag(Mark mark)
    {
        return mark.Type switch
        {
            "bold" => "strong",
            "underline" => "u",
            "italic" => "em",
            "code" => "code",
            _ => "span"
        };
    }
}