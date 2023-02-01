using System.Threading.Tasks;
using Blog.Features.CodeBlock.Models;

namespace Blog.Features.CodeBlock;

public interface ICodeBlockLoader
{
    Task<CodeBlockContent> GetCodeBlock(string slug);
}
