namespace Blog.Features.CodeBlock.Models;

public class CodeBlockViewModel
{
    public CodeBlockViewModel(CodeBlockContent content)
    {
        Title = content.Title;
        Slug = content.Slug;
        Language = content.Language ?? string.Empty;
        Code = content.CodeString ?? string.Empty;
    }

    public string Title { get; set; }
    public string Slug { get; set; }
    public string Language { get; set; }
    public string Code { get; set; }
}