using Contentful.Core.Models;

namespace Core.Features.Image;

public class ImageViewModel
{
    // File properties
    public string Url { get; set; }
    public string FileName { get; set; }
    public long Size { get; set; }

    // Image properties
    public string AltText { get; set; }
    public string Caption { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int OriginalWidth { get; set; }
    public int OriginalHeight { get; set; }

    // Link
    public string LinkUrl { get; set; }

    // View settings
    public bool ShowCaption { get; set; }

    public ImageViewModel(Asset image, bool showCaption = false)
    {
        Url = image.File.Url;
        FileName = image.File.FileName;
        Size = image.File.Details.Size;

        AltText = image.Title;
        Caption = image.Description;
        Width = image.File.Details.Image.Width;
        Height = image.File.Details.Image.Height;
        OriginalWidth = image.File.Details.Image.Width;
        OriginalHeight = image.File.Details.Image.Height;

        ShowCaption = showCaption;
    }
}