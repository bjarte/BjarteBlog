using Blog.Features.AppSettings;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Features.Caching;

[Route("api/[controller]")]
[ApiController]
public class ClearCacheController : Controller
{
    private readonly IAppSettingsService _appSettingsService;
    private readonly IOutputCachingService _outputCachingService;

    public ClearCacheController(
        IAppSettingsService appSettingsService,
        IOutputCachingService outputCachingService
    )
    {
        _appSettingsService = appSettingsService;
        _outputCachingService = outputCachingService;
    }

    [HttpGet("{secret}")]
    public IActionResult Index(string secret)
    {
        if (string.IsNullOrEmpty(secret)
            || secret.Length < 10)
        {
            return new BadRequestResult();
        }

        if (secret != _appSettingsService.GetCacheKey())
        {
            return new BadRequestResult();
        }

        _outputCachingService.Clear();

        return new JsonResult("Cache cleared");
    }
}