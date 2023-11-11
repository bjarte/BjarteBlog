using System.Threading.Tasks;
using Blog.Features.Page.Models;

namespace Blog.Features.Page;

public interface IPageLoader
{
    Task<PageContent> Get(string slug);
    Task<string> GetSlug(string id);
    Task<PageContent> GetPreview(string id);
}