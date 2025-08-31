namespace Blog.Features.Editorial;

public interface IPreviewLoader
{
    Task<T> GetPreview<T>(string id) where T : EditorialContent;
}

