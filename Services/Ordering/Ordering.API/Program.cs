using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extencions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServer:Authority"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            NameClaimType = "name",
            RoleClaimType = "role"
        };
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    await app.InitialDatabaseAsync();
}

app.Run();