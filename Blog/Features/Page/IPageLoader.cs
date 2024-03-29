﻿namespace Blog.Features.Page;

public interface IPageLoader
{
    Task<PageContent> Get(string slug);
    Task<string> GetSlug(string id);
    Task<PageContent> GetPreview(string id);
}