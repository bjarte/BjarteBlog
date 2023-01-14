namespace Core.Features.Editorial;

public interface IRichTextLoader
{
    string BodyToHtml(IHasBody content);
}