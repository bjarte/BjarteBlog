using System.Threading.Tasks;
using Blog.Features.Navigation.Models;

namespace Blog.Features.Navigation;

public interface INavigationLoader
{
    Task<NavigationContent> GetNavigation();
}
