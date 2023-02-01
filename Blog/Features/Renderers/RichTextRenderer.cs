using Contentful.Core.Models;

namespace Blog.Features.Renderers;

public class RichTextRenderer : IRichTextRenderer
{
    private readonly ICodeBlockContentRenderer _codeBlockContentRenderer;

    public RichTextRenderer(ICodeBlockContentRenderer codeBlockContentRenderer)
    {
        _codeBlockContentRenderer = codeBlockContentRenderer;
    }

    public string BodyToHtml(IHasBody content)
    {
        if (content?.Body?.Content?.Any() != true)
        {
            return string.Empty;
        }

        var htmlRenderer = new HtmlRenderer();
        htmlRenderer.AddRenderer(new AdvancedTextRenderer());
        htmlRenderer.AddRenderer(_codeBlockContentRenderer);

        var html = htmlRenderer.ToHtml(content.Body).Result;

        return html;
    }
}