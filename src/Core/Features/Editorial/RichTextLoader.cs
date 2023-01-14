using System.Linq;
using Contentful.Core.Models;
using Core.Features.CodeBlock;

namespace Core.Features.Editorial;

public class RichTextLoader : IRichTextLoader
{
    private readonly ICodeBlockContentRenderer _codeBlockContentRenderer;

    public RichTextLoader(ICodeBlockContentRenderer codeBlockContentRenderer)
    {
        _codeBlockContentRenderer = codeBlockContentRenderer;
        _codeBlockContentRenderer.Order = 10;
    }

    public string BodyToHtml(IHasBody content)
    {
        if (content?.Body?.Content?.Any() != true)
        {
            return string.Empty;
        }

        var htmlRenderer = new HtmlRenderer();
        htmlRenderer.AddRenderer(_codeBlockContentRenderer);

        var html = htmlRenderer.ToHtml(content.Body).Result;

        return html;
    }
}