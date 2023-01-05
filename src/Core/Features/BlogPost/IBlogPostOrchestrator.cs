using System.Collections.Generic;
using Core.Features.BlogPost.Models;

namespace Core.Features.BlogPost;

public interface IBlogPostOrchestrator
{
    public IEnumerable<BlogPostViewModel> GetBlogPosts(string id, bool preview, out string title);

}