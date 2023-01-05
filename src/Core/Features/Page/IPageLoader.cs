using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Features.Page.Models;

namespace Core.Features.Page;

public interface IPageLoader
{
    Task<PageContent> GetPage(string slug);
    Task<PageContent> GetPagePreview(string id);
    Task<IEnumerable<PageContent>> GetPages();
}