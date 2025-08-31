namespace Blog.Features.CodeBlock.Models;

public class CodeBlockContent : ContentBase
{
    public string Language { get; set; }
    public Document Code { get; set; }
    public string CodeString { get; set; }
}