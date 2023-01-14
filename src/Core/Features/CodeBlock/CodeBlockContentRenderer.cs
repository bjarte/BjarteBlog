using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using Core.Features.CodeBlock.Models;

namespace Core.Features.CodeBlock
{
    public class CodeBlockContentRenderer : ICodeBlockContentRenderer
    {
        public int Order { get; set; }

        private readonly IContentfulClient _contentDeliveryClient;

        public CodeBlockContentRenderer(
            IContentfulClient contentDeliveryClient
        )
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

            var id = customNode.JObject.Value<string>("$id");

            CodeBlockContent codeBlockContent;

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

            var temp = codeBlockContent.Code.Content[0];

            var htmlRenderer = new HtmlRenderer();
            var html = htmlRenderer.ToHtml(codeBlockContent.Code).Result;

            var sb = new StringBuilder();

            sb.Append($"<pre><code class=\"language-{codeBlockContent.Language}\">");

            sb.Append($"{html}");

            sb.Append("</code></pre>");

            return sb.ToString();
        }
    }
}
