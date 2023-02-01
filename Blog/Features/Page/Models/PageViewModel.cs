using Blog.Features.Image;

namespace Blog.Features.Page.Models;

public class PageViewModel : BasePageViewModel
{
    public string Body { get; set; }
    public ImageViewModel Image { get; set; }

    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public PageViewModel(PageContent content, bool showImageCaption = false)
    {
        if (content == null)
        {
            return;
        }

        Title = content.Title ?? string.Empty;
        Slug = content.Slug ?? string.Empty;
        Body = content.BodyString ?? string.Empty;

        if (content.MainImage != null)
        {
            Image = new ImageViewModel(content.MainImage, showImageCaption);
        }

        CreatedAt = content.Sys?.CreatedAt;
        UpdatedAt = content.Sys?.UpdatedAt;
    }
}