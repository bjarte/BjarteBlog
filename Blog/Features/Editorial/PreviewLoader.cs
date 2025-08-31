namespace Blog.Features.Editorial;

public class PreviewLoader : IPreviewLoader
{
    // The Preview API is for previewing unpublished content as though
    // it were published. It maintains the same behaviour and parameters
    // as the CDA, but delivers the latest draft for entries and assets.
    private readonly ContentfulClient _previewClient;
    private readonly IRichTextRenderer _richTextRenderer;

    public PreviewLoader(
        IOptions<ContentfulConfig> contentfulConfig,
        IRichTextRenderer richTextRenderer
    )
    {
        var contentfulOptions = contentfulConfig.Value.ToContentfulOptions();
        contentfulOptions.UsePreviewApi = true;
        _previewClient = new ContentfulClient(new HttpClient(), contentfulOptions);
        _richTextRenderer = richTextRenderer;
    }

    public async Task<T> GetPreview<T>(string id) where T : EditorialContent
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        string contentType;
        if (typeof(T) == typeof(BlogPostContent))
        {
            contentType = ContentTypes.BlogPost;
        }
        else if (typeof(T) == typeof(PageContent))
        {
            contentType = ContentTypes.Page;
        }
        else
        {
            throw new NotSupportedException($"Content type {typeof(T).Name} is not supported for preview.");
        }

        var query = new QueryBuilder<T>()
            .ContentTypeIs(contentType)
            .FieldEquals(content => content.Sys.Id, id)
            .Include(2);

        var blogPost = (await _previewClient
                .GetEntries(query))
            .FirstOrDefault();

        if (blogPost == null)
        {
            return null;
        }

        blogPost.BodyString = _richTextRenderer.BodyToHtml(blogPost);

        return blogPost;
    }
}