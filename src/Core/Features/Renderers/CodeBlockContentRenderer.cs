using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using Core.Features.CodeBlock.Models;

namespace Core.Features.Renderers
{
    public class CodeBlockContentRenderer : ICodeBlockContentRenderer
    {
        public int Order { get; set; } = 20;

        private readonly IContentfulClient _contentDeliveryClient;

        public CodeBlockContentRenderer(IContentfulClient contentDeliveryClient)
        {
            _contentDeliveryClient = contentDeliveryClient;
        }

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
                codeBlockContent = await _contentDeliveryClient
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
}
