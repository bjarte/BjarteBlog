using Contentful.Core.Configuration;

namespace Core.Features.AppSettings;

public interface IAppSettingsService
{
    ContentfulOptions GetContentfulOptions();

    string GetContentfulNavigation();
}