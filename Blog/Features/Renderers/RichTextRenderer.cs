namespace Blog.Features.Renderers;

public class RichTextRenderer(
    ICodeBlockContentRenderer codeBlockContentRenderer
) : IRichTextRenderer
{
    public string BodyToHtml(EditorialContent content)
    {
        if (content?.Body?.Content?.Count > 0 != true)
        {
            return string.Empty;
        }

        var htmlRenderer = new HtmlRenderer();
        htmlRenderer.AddRenderer(new AdvancedTextRenderer());
        htmlRenderer.AddRenderer(codeBlockContentRenderer);

        return htmlRenderer.ToHtml(content.Body).Result;
    }
}