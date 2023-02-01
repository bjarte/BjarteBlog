using Contentful.Core.Configuration;

namespace Blog.Features.AppSettings;

public interface IAppSettingsService
{
    ContentfulOptions GetContentfulOptions();

    string GetContentfulNavigation();
}