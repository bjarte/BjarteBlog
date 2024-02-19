namespace Blog.Features.CodeBlock.Models;

public class CodeBlockContent : ContentfulContent
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Language { get; set; }
    public Document Code { get; set; }
    public string CodeString { get; set; }
}