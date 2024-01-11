using Blog.Features.AppSettings;
using WebEssentials.AspNetCore.OutputCaching;

namespace Blog.Features.Caching;

[Route("api/[controller]")]
[ApiController]
public class ClearCacheController(IAppSettingsService appSettingsService, IOutputCachingService outputCachingService) : Controller
{
    [HttpGet("{secret}")]
    public IActionResult Index(string secret)
    {
        if (string.IsNullOrEmpty(secret)
            || secret.Length < 10)
        {
            return new BadRequestResult();
        }

        if (secret != appSettingsService.GetCacheKey())
        {
            return new BadRequestResult();
        }

        outputCachingService.Clear();

        return new JsonResult("Cache cleared");
    }
}