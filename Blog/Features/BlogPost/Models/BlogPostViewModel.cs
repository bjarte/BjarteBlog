using Blog.Features.Category.Models;
using Blog.Features.Image;
using Blog.Features.Page.Models;

namespace Blog.Features.BlogPost.Models;

public class BlogPostViewModel : BasePageViewModel
{
    public string Intro { get; set; }
    public string Body { get; set; }

    public ImageViewModel Image { get; set; }

    public List<CategoryViewModel> Categories { get; set; }

    public string TypeformFormId { get; set; }

    public DateTime? PublishedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public BlogPostViewModel(BlogPostContent content, bool showImageCaption = false)
    {
        if (content == null)
        {
            return;
        }

        Title = content.Title ?? string.Empty;
        Slug = content.Slug ?? string.Empty;
        Intro = content.Intro ?? string.Empty;
        Body = content.BodyString ?? string.Empty;

        if (content.MainImage != null)
        {
            Image = new ImageViewModel(content.MainImage, showImageCaption);
        }

        Categories = content.Categories?.Select(
            _ => new CategoryViewModel(_)).ToList();

        TypeformFormId = content.TypeformFormId;

        PublishedAt = content.PublishedAt;
        CreatedAt = content.Sys?.CreatedAt;
        UpdatedAt = content.Sys?.UpdatedAt;
    }
}