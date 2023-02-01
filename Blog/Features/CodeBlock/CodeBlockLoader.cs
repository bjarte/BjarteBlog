using System.Threading.Tasks;
using Blog.Features.CodeBlock.Models;
using Contentful.Core;
using Contentful.Core.Search;

namespace Blog.Features.CodeBlock;

public class CodeBlockLoader : ICodeBlockLoader
{
    private readonly IContentfulClient _contentDeliveryClient;

    public CodeBlockLoader(IContentfulClient contentDeliveryClient)
    {
        _contentDeliveryClient = contentDeliveryClient;
    }

    public async Task<CodeBlockContent> GetCodeBlock(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var query = new QueryBuilder<CodeBlockContent>()
            .ContentTypeIs("codeBlock")
            .FieldEquals(_ => _.Slug, slug);

        var entries = await _contentDeliveryClient
            .GetEntries(query);

        return entries.FirstOrDefault();
    }
}