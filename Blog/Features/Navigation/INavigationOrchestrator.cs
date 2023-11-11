using Blog.Features.Navigation.Models;

namespace Blog.Features.Navigation
{
    public interface INavigationOrchestrator
    {
        public NavigationViewModel Get();
    }
}
