using System.Threading.Tasks;
using Blog.Features.Page.Models;

namespace Blog.Features.Page;

public interface IPageLoader
{
    Task<PageContent> GetPage(string slug);
    Task<PageContent> GetPagePreview(string id);
    Task<IEnumerable<PageContent>> GetPages();
}