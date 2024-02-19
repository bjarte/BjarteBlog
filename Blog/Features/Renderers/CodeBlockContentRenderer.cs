namespace Blog.Features.Renderers;

public class CodeBlockContentRenderer(
    IContentfulClient contentDeliveryClient
) : ICodeBlockContentRenderer
{
    public int Order { get; set; } = 20;

    public bool SupportsContent(IContent content)
    {
        return content is EntryStructure { NodeType: "embedded-entry-block" };
    }

    public async Task<string> RenderAsync(IContent content)
    {
        if ((content as EntryStructure)?.Data.Target is not CustomNode customNode)
        {
            return string.Empty;
        }

        CodeBlockContent codeBlockContent;

        var id = customNode.JObject.Value<string>("$id");
        try
        {
            codeBlockContent = await contentDeliveryClient
                .GetEntry<CodeBlockContent>(id);
        }
        catch (Exception)
        {
            return string.Empty;
        }

        if (codeBlockContent?.Code?.Content?.Any() != true)
        {
            return string.Empty;
        }

        var htmlRenderer = new HtmlRenderer();
        var code = htmlRenderer.ToHtml(codeBlockContent.Code).Result;

        var html = new StringBuilder();

        html.Append($"<pre><code class=\"language-{codeBlockContent.Language}\">");
        html.Append($"{code}");
        html.Append("</code></pre>");

        return html.ToString();
    }
}