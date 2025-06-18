using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Refit;
using Shopping.Web.Handlers;
using Shopping.Web.Services;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure Data Protection with persistent storage
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/home/app/.aspnet/DataProtection-Keys"))
    .SetApplicationName("EShopOnContainers");

// Add HttpContextAccessor for authentication handler
builder.Services.AddHttpContextAccessor();

// Configure JWT handling
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

// Configure Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie.Name = "shopping.web";
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    var authority = builder.Configuration["IdentityServer:Authority"];
    var publicAuthority = builder.Configuration["IdentityServer:PublicAuthority"];

    options.Authority = authority;
    options.MetadataAddress = $"{authority}/.well-known/openid-configuration";

    options.ClientId = "shopping.web";
    options.ResponseType = OpenIdConnectResponseType.Code;
    options.UsePkce = true;

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("roles");
    options.Scope.Add("basket.api");
    options.Scope.Add("catalog.api");
    options.Scope.Add("ordering.api");
    options.Scope.Add("gateway");
    options.Scope.Add("offline_access");

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.RequireHttpsMetadata = false;

    // Map claims
    options.ClaimActions.MapJsonKey("role", "role");
    options.ClaimActions.MapJsonKey("name", "name");
    options.ClaimActions.MapJsonKey("email", "email");

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };

    // Configure callback path
    options.CallbackPath = "/signin-oidc";
    options.SignedOutCallbackPath = "/signout-callback-oidc";
    options.RemoteSignOutPath = "/signout-oidc";

    // Handle events
    options.Events = new OpenIdConnectEvents
    {
        OnRedirectToIdentityProvider = context =>
        {
            // Use public authority for browser redirects
            if (!string.IsNullOrEmpty(publicAuthority))
            {
                context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.Replace(authority, publicAuthority);
            }

            return Task.CompletedTask;
        },
        OnRemoteFailure = context =>
        {
            context.Response.Redirect("/");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnSignedOutCallbackRedirect = context =>
        {
            context.Response.Redirect("/");
            context.HandleResponse();
            return Task.CompletedTask;
        },
        OnRedirectToIdentityProviderForSignOut = context =>
        {
            context.Properties.RedirectUri = "/";
            return Task.CompletedTask;
        },
        OnTokenResponseReceived = context =>
        {
            // ѕосле получени€ токенов, убеждаемс€ что перенаправление идет на главную
            if (string.IsNullOrEmpty(context.Properties?.RedirectUri))
            {
                context.Properties.RedirectUri = "/";
            }
            return Task.CompletedTask;
        }
    };

    // For development - accept any certificate
    options.BackchannelHttpHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Configure Refit clients with authentication
builder.Services.AddTransient<AuthenticationDelegatingHandler>();

builder.Services
    .AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services
    .AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services
    .AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// ƒобавл€ем маршрут по умолчанию дл€ signin-oidc
app.Map("/signin-oidc", async context =>
{
    // Ёто будет обработано OpenID Connect middleware
    // ѕосле обработки перенаправл€ем на главную
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        context.Response.Redirect("/");
    }
});

app.Run();