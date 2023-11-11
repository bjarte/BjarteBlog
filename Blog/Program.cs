using System.Globalization;
using Blog.Features.AppSettings;
using Blog.Features.BlogPost;
using Blog.Features.Category;
using Blog.Features.Navigation;
using Blog.Features.Page;
using Blog.Features.Renderers;
using Contentful.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Dependency injection
builder.Services.AddSingleton<IAppSettingsService, AppSettingsService>();
builder.Services.AddSingleton<IBlogPostLoader, BlogPostLoader>();
builder.Services.AddSingleton<IBlogPostOrchestrator, BlogPostOrchestrator>();
builder.Services.AddSingleton<ICategoryLoader, CategoryLoader>();
builder.Services.AddSingleton<ICategoryOrchestrator, CategoryOrchestrator>();
builder.Services.AddSingleton<ILinkLoader, LinkLoader>();
builder.Services.AddSingleton<INavigationOrchestrator, NavigationOrchestrator>();
builder.Services.AddSingleton<IPageLoader, PageLoader>();

builder.Services.AddSingleton<ICodeBlockContentRenderer, CodeBlockContentRenderer>();
builder.Services.AddSingleton<IRichTextRenderer, RichTextRenderer>();

// Add services to the container
builder.Services.AddOutputCaching();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddContentful(builder.Configuration);
builder.Services.AddLocalization();

var app = builder.Build();

var supportedCultures = new[] { new CultureInfo("en-GB") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-GB"),
    SupportedCultures = supportedCultures,
    FallBackToParentCultures = false
});
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-GB");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseOutputCaching();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.MapRazorPages();

app.Run();