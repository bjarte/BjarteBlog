﻿namespace Blog.Features.Caching;

[Route("api/[controller]")]
[ApiController]
public class ClearCacheController(
    IOptions<OutputCacheConfig> outputCacheConfig,
    IOutputCachingService outputCachingService
) : Controller
{
    private readonly OutputCacheConfig _outputCacheConfig = outputCacheConfig.Value;

    [HttpGet("{secret}")]
    public IActionResult Index(string secret)
    {
        if (string.IsNullOrEmpty(secret)
            || secret.Length < 10)
        {
            return new BadRequestResult();
        }

        if (secret != _outputCacheConfig.CacheKey)
        {
            return new BadRequestResult();
        }

        outputCachingService.Clear();

        return new JsonResult("Cache cleared");
    }
}