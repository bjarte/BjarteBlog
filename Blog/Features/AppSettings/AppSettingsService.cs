using System.Configuration;
using Contentful.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Blog.Features.AppSettings;

public class AppSettingsService(
        IConfiguration configuration,
        ILogger logger
    ) : IAppSettingsService
{
    private const string ContentfulOptions = nameof(ContentfulOptions);
    private const string OutputCacheSettings = nameof(OutputCacheSettings);

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

    public string GetCacheKey()
    {
        return GetString($"{OutputCacheSettings}:CacheKey", true);
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
        var setting = configuration[appSettingKey];

        if (!string.IsNullOrWhiteSpace(setting))
        {
            return setting;
        }

        logger.LogError($"AppSetting '{appSettingKey}' is undefined");

        if (required)
        {
            throw new SettingsPropertyNotFoundException($"Required appsetting '{appSettingKey}' is undefined");
        }

        return defaultValue;
    }

    #endregion Helper methods
}