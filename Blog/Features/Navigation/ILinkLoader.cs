namespace Blog.Features.Navigation;

public interface ILinkLoader
{
    Task<IEnumerable<LinkContent>> Get();
}
