namespace Blog.Features.CodeBlock;

public interface ICodeBlockLoader
{
    Task<CodeBlockContent> Get(string slug);
}
