using Blog.Features.Category.Models;
using Xunit;

namespace Blog.Tests;

public class CategoryTests
{
    [Fact]
    public void CategoryViewModelConstructor_WhenContentIsNull_ReturnsValidViewModel()
    {
        var model = new CategoryViewModel(null);

        Assert.NotEmpty(model.Title);
        Assert.NotEmpty(model.Slug);
    }
}