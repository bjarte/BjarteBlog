namespace Blog.Features.Image;

public class ImageViewModel(Asset image, bool showCaption = false)
{
    // File properties
    public string Url { get; set; } = image.File.Url;
    public string FileName { get; set; } = image.File.FileName;
    public long Size { get; set; } = image.File.Details.Size;

    // Image properties
    public string AltText { get; set; } = image.Title;
    public string Caption { get; set; } = image.Description;
    public int Width { get; set; } = image.File.Details.Image.Width;
    public int Height { get; set; } = image.File.Details.Image.Height;
    public int OriginalWidth { get; set; } = image.File.Details.Image.Width;
    public int OriginalHeight { get; set; } = image.File.Details.Image.Height;

    // Link
    public string LinkUrl { get; set; }

    // View settings
    public bool ShowCaption { get; set; } = showCaption;
}