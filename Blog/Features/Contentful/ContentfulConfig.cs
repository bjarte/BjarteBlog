
namespace Blog.Features.Contentful;

public class ContentfulConfig
{
    [Required]
    public string SpaceId { get; set; }
    [Required]
    public string DeliveryApiKey { get; set; }
    [Required]
    public string ManagementApiKey { get; set; }
    [Required]
    public string PreviewApiKey { get; set; }
    [Required]
    public string Environment { get; set; }
    [Required]
    public string Navigation { get; set; }
}