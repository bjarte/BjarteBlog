namespace Blog.Features.CodeBlock.Models;

public class CodeBlockViewModel(CodeBlockContent content)
{
    public string Title { get; set; } = content.Title;
    public string Slug { get; set; } = content.Slug;
    public string Language { get; set; } = content.Language ?? string.Empty;
    public string Code { get; set; } = content.CodeString ?? string.Empty;
}