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
            new ApiScope("ordering.api", "Ordering API")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("catalog.api", "Catalog API")
            {
                Scopes = { "catalog.api" }
            },
            new ApiResource("basket.api", "Basket API")
            {
                Scopes = { "basket.api" }
            },
            new ApiResource("discount.api", "Discount API")
            {
                Scopes = { "discount.api" }
            },
            new ApiResource("ordering.api", "Ordering API")
            {
                Scopes = { "ordering.api" }
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

                RedirectUris = { "https://localhost:6065/signin-oidc" },
                PostLogoutRedirectUris = { "https://localhost:6065/signout-callback-oidc" },
                AllowedCorsOrigins = { "https://localhost:6065" },

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
                    "ordering.api"
                }
            },

            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:44300/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "scope2" }
            },
        };
}
