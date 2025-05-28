using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity.API;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource("roles", "User roles", new[] { "role" })
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("catalog.api", "Catalog API"),
            new ApiScope("basket.api", "Basket API"),
            new ApiScope("discount.api", "Discount API"),
            new ApiScope("ordering.api", "Ordering API"),
            new ApiScope("gateway", "API Gateway")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("catalog.api", "Catalog API")
            {
                Scopes = { "catalog.api" },
                UserClaims = { "name", "email", "role" }
            },
            new ApiResource("basket.api", "Basket API")
            {
                Scopes = { "basket.api" },
                UserClaims = { "name", "email", "role" }
            },
            new ApiResource("discount.api", "Discount API")
            {
                Scopes = { "discount.api" },
                UserClaims = { "name", "email", "role" }
            },
            new ApiResource("ordering.api", "Ordering API")
            {
                Scopes = { "ordering.api" },
                UserClaims = { "name", "email", "role" }
            },
            new ApiResource("gateway", "API Gateway")
            {
                Scopes = { "gateway" },
                UserClaims = { "name", "email", "role" }
            }
        };

    public static IEnumerable<Client> Clients =>
    new Client[]
    {
        new Client
        {
            ClientId = "shopping.web",
            ClientName = "Shopping Web App",
            AllowedGrantTypes = GrantTypes.Code,
            RequireClientSecret = false,
            RequirePkce = true,

            RedirectUris = {
                "https://localhost:6065/signin-oidc",
                "http://localhost:6005/signin-oidc",
                "https://shopping.web:8081/signin-oidc",
                "http://shopping.web:8080/signin-oidc"
            },
            PostLogoutRedirectUris = {
                "https://localhost:6065/signout-callback-oidc",
                "http://localhost:6005/signout-callback-oidc",
                "https://shopping.web:8081/signout-callback-oidc",
                "http://shopping.web:8080/signout-callback-oidc"
            },
            AllowedCorsOrigins = {
                "https://localhost:6065",
                "http://localhost:6005",
                "https://shopping.web:8081",
                "http://shopping.web:8080"
            },

            AllowOfflineAccess = true,
            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                IdentityServerConstants.StandardScopes.Email,
                "roles",
                "catalog.api",
                "basket.api",
                "discount.api",
                "ordering.api",
                "gateway"
            },
            AlwaysIncludeUserClaimsInIdToken = true,
            AccessTokenLifetime = 3600,
            IdentityTokenLifetime = 3600
        }
    };
}