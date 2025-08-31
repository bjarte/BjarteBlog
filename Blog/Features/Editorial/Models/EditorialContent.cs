namespace Blog.Features.Editorial.Models;

public abstract class EditorialContent : ContentBase, IHasBody
{
    public Document Body { get; set; }
    public string BodyString { get; set; }
}