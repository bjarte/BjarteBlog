using Contentful.Core.Errors;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Features.CodeBlock;

public class CodeBlockLoader(
    IContentfulClient contentDeliveryClient,
    IMemoryCache cache
) : ICodeBlockLoader
{
    public async Task<CodeBlockContent> Get(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        var cacheKey = $"contentful_codeblock_id_{id}";
        if (cache.TryGetValue(cacheKey, out CodeBlockContent cachedCodeBlock))
        {
            return cachedCodeBlock;
        }

        var query = new QueryBuilder<CodeBlockContent>()
            .ContentTypeIs(ContentTypes.CodeBlock)
            .FieldEquals(codeBlock => codeBlock.Slug, id);

        try
        {
            var codeBlock = (await contentDeliveryClient
                    .GetEntries(query))
                .FirstOrDefault();

            if (codeBlock == null)
            {
                return null;
            }

            cache.Set(cacheKey, codeBlock);

            return codeBlock;
        }
        catch (ContentfulException)
        {
            return null;
        }
    }
}