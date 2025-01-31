namespace Blog.Features.Navigation;

public interface INavigationOrchestrator
{
    public Task<NavigationViewModel> Get();
}