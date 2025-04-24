using Microsoft.EntityFrameworkCore;
using Sec.Market.API.Data;
using Sec.Market.API.Interfaces;
using Sec.Market.API.Repository;
using Sec.Market.API.Services;
using Stripe;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<MarketContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPaiementService, PaiementService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPaiementRepository, PaiementRepository>();
builder.Services.AddScoped<ICustomerReviewRepository, CustomerReviewRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo Auth API", Version = "v1" });
    option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(builder.Configuration.GetSection("AzureAd")["AuthorizationUrl"]),
                TokenUrl = new Uri(builder.Configuration.GetSection("AzureAd")["TokenUrl"]),
                Scopes = new Dictionary<string, string>
                {

                    {

                    builder.Configuration.GetSection("AzureAd")["ApiTokenScope"], "API Access"

                    }

                }
            }
        }
    });
    // Cette section est cruciale pour que Swagger envoie le token
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { builder.Configuration["AzureAd:ApiTokenScope"] }
        }
    });
});


var app = builder.Build();

    StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("StripeAPIKeys");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(option =>
        {
            option.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo Auth API v1");

            option.OAuthClientId(builder.Configuration["AzureAd:ClientId"]);
            option.OAuthUsePkce();
            option.OAuthScopeSeparator(" ");
            option.OAuthScopes(builder.Configuration["AzureAd:ApiTokenScope"]);

            //option.OAuthClientId(builder.Configuration.GetSection("AzureAd")["ClientId"]);
            //option.OAuthUsePkce();
            //option.OAuthScopeSeparator(" ");
            //option.OAuthScopes(builder.Configuration["AzureAd:ApiTokenScope"]);
        });
    }

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
