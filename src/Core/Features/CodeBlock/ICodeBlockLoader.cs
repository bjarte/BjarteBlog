using System.Threading.Tasks;
using Core.Features.CodeBlock.Models;

namespace Core.Features.CodeBlock;

public interface ICodeBlockLoader
{
    Task<CodeBlockContent> GetCodeBlock(string slug);
}
