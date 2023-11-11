using System.Threading.Tasks;
using Blog.Features.Navigation.Models;

namespace Blog.Features.Navigation;

public interface ILinkLoader
{
    Task<IEnumerable<LinkContent>> Get();
}
