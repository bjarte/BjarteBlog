namespace Blog.Features.Renderers;

public interface IRichTextRenderer
{
    string BodyToHtml(IHasBody content);
}