namespace Blog.Features.Editorial;

public class PreviewLoader : IPreviewLoader
{
    private const string BlogPostContentType = "blogpost";

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

        var query = new QueryBuilder<T>()
            .ContentTypeIs(BlogPostContentType)
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