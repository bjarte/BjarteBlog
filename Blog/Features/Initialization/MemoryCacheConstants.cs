namespace Blog.Features.Initialization;

public static class MemoryCacheConstants
{
    public static readonly MemoryCacheEntryOptions SlidingExpiration1Day = new()
    {
        SlidingExpiration = TimeSpan.FromDays(1)
    };
}