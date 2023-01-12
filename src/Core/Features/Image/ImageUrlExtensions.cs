using System.Collections.Generic;

namespace Core.Features.Image;

public static class ImageExtensions
{
    public static string GetUrl(this ImageViewModel image, int width = 0, int height = 0)
    {
        // Change format to WEBP, set crop mode to "fill"
        var url = $"{image.Url}?fm=webp&fit=fill&q=80";

        // Resize to width
        if (width > 0)
        {
            url += $"&w={width}";
        }

        // Resize to height
        if (height > 0)
        {
            url += $"&h={height}";
        }

        return url;
    }

    public static IEnumerable<ImageSourceViewModel> GetImageSources(
        this ImageViewModel image, int width = 0, int height = 0)
    {
        var divisors = new[] { 4, 3, 2, 1 };

        foreach (var divisor in divisors)
        {
            var sourceWidth = width / divisor;
            var sourceHeight = height / divisor;
            var sourceUrl = image.GetUrl(sourceWidth,
                sourceHeight);
            var sourceUrl2X = image.GetUrl(sourceWidth * 2,
                sourceHeight * 2);

            var media = $"(max-width: {sourceWidth}px)";
            var srcSet = $"{sourceUrl}, {sourceUrl2X} 2x";

            yield return new ImageSourceViewModel
            {
                Media = media,
                SrcSet = srcSet
            };
        }
    }

    public static string GetUrlWithAttribute(this ImageViewModel image, int width = 0, int height = 0)
    {
        return $"{image.GetUrl(width, height)} {width}w";
    }

    public static int GetHeight(this ImageViewModel image, int width)
    {
        return image.OriginalHeight * width / image.OriginalWidth;
    }
}
