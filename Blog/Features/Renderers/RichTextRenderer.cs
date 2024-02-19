namespace Blog.Features.Renderers;

public class RichTextRenderer(ICodeBlockContentRenderer codeBlockContentRenderer) : IRichTextRenderer
{
    public string BodyToHtml(IHasBody content)
    {
        if (content?.Body?.Content?.Any() != true)
        {
            return string.Empty;
        }

        var htmlRenderer = new HtmlRenderer();
        htmlRenderer.AddRenderer(new AdvancedTextRenderer());
        htmlRenderer.AddRenderer(codeBlockContentRenderer);

        return htmlRenderer.ToHtml(content.Body).Result;
    }
}