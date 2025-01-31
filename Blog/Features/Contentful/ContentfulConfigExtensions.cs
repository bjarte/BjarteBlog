using Contentful.Core.Configuration;

namespace Blog.Features.Contentful;

public static class ContentfulConfigExtensions
{
    public static ContentfulOptions ToContentfulOptions(this ContentfulConfig config)
    {
        return new ContentfulOptions
        {
            SpaceId = config.SpaceId,
            DeliveryApiKey = config.DeliveryApiKey,
            ManagementApiKey = config.ManagementApiKey,
            PreviewApiKey = config.PreviewApiKey,
            Environment = config.Environment,

            MaxNumberOfRateLimitRetries = 0,
            UsePreviewApi = false
        };
    }
}