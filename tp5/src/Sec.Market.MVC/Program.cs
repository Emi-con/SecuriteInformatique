using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Sec.Market.MVC.Handlers;
using Sec.Market.MVC.Interfaces;
using Sec.Market.MVC.Services;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');

// Configuration d'Azure AD � partir de la configuration
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
            .AddDownstreamApi("DownstreamApi",builder.Configuration.GetSection("DownstreamApi"))
            .AddInMemoryTokenCaches();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddRazorPages().AddMicrosoftIdentityUI();

builder.Services.AddSingleton<TokenService>();
builder.Services.AddTransient<BearerTokenHandler>();

var apiBaseUrl = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);

builder.Services.AddHttpClient<IProductService, ProductServiceProxy>(client =>
{
    client.BaseAddress = apiBaseUrl;
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IUserService, UserServiceProxy>(client =>
{
    client.BaseAddress = apiBaseUrl;
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<IOrderService, OrderServiceProxy>(client =>
{
    client.BaseAddress = apiBaseUrl;
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddHttpClient<ICustomerReviewService, CustomerReviewServiceProxy>(client =>
{
    client.BaseAddress = apiBaseUrl;
}).AddHttpMessageHandler<BearerTokenHandler>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Auth";
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
