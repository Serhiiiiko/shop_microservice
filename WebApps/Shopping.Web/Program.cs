using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Shopping.Web.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = builder.Configuration["IdentityServer:Authority"];
    options.ClientId = "shopping.web";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.RequireHttpsMetadata = false;

    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("roles");
    options.Scope.Add("catalog.api");
    options.Scope.Add("basket.api");
    options.Scope.Add("ordering.api");
    options.Scope.Add("gateway");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "role"
    };
});

builder.Services.AddTransient<AuthenticationDelegatingHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]!);
    })
    .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();