using System.Threading.Tasks;
using Blog.Features.CodeBlock.Models;
using Contentful.Core;
using Contentful.Core.Search;

namespace Blog.Features.CodeBlock;

public class CodeBlockLoader(IContentfulClient contentDeliveryClient) : ICodeBlockLoader
{
    public async Task<CodeBlockContent> Get(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var query = new QueryBuilder<CodeBlockContent>()
            .ContentTypeIs("codeBlock")
            .FieldEquals(_ => _.Slug, slug);

        var entries = await contentDeliveryClient
            .GetEntries(query);

        return entries.FirstOrDefault();
    }
}