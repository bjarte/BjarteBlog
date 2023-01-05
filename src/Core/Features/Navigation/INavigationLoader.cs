using System.Threading.Tasks;
using Core.Features.Navigation.Models;

namespace Core.Features.Navigation;

public interface INavigationLoader
{
    Task<NavigationContent> GetNavigation();
}
