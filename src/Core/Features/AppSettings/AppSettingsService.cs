using System.Configuration;
using Contentful.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Core.Features.AppSettings;

public class AppSettingsService : IAppSettingsService
{
    private const string ContentfulOptions = "ContentfulOptions";

    private readonly IConfiguration _configuration;
    private readonly ILogger<AppSettingsService> _logger;

    public AppSettingsService(
        IConfiguration configuration,
        ILogger<AppSettingsService> logger
    )
    {
        _configuration = configuration;
        _logger = logger;
    }

    public ContentfulOptions GetContentfulOptions()
    {
        return new ContentfulOptions
        {
            SpaceId = GetString($"{ContentfulOptions}:SpaceId", true),
            DeliveryApiKey = GetString($"{ContentfulOptions}:DeliveryApiKey", true),
            ManagementApiKey = GetString($"{ContentfulOptions}:ManagementApiKey", true),
            PreviewApiKey = GetString($"{ContentfulOptions}:PreviewApiKey", true),
            Environment = GetString($"{ContentfulOptions}:Environment"),

            MaxNumberOfRateLimitRetries = 0,
            UsePreviewApi = false,
            ResolveEntriesSelectively = false
        };
    }

    public string GetContentfulNavigation()
    {
        return GetContentfulOption("Navigation");
    }

    #region Helper methods

    private string GetContentfulOption(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return string.Empty;
        }

        return GetString($"{ContentfulOptions}:{name}") ?? string.Empty;
    }

    private string GetString(string appSettingKey, bool required = false, string defaultValue = "")
    {
        var setting = _configuration[appSettingKey];

        if (!string.IsNullOrWhiteSpace(setting))
        {
            return setting;
        }

        _logger.LogError($"AppSetting '{appSettingKey}' is undefined");

        if (required)
        {
            throw new SettingsPropertyNotFoundException($"Required appsetting '{appSettingKey}' is undefined");
        }

        return defaultValue;
    }

    #endregion Helper methods
}