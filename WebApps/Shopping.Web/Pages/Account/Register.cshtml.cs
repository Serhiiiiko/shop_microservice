using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Shopping.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly IConfiguration _configuration;

    public RegisterModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult OnGet(string returnUrl = "/")
    {
        // Ensure we have a proper return URL
        if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/Account/Register")
        {
            returnUrl = "/";
        }

        // Get URLs from configuration
        var identityServerUrl = _configuration["IdentityServer:Authority"];
        var publicUrl = _configuration["IdentityServer:PublicAuthority"] ?? identityServerUrl;

        // Get the current host URL
        var request = HttpContext.Request;
        var currentHost = request.Host.Value;
        var scheme = request.Scheme;

        // Build the callback URL that Identity Server should use after registration
        var callbackUrl = $"{scheme}://{currentHost}/signin-oidc";

        // Encode the full return path
        var fullReturnUrl = Uri.EscapeDataString(callbackUrl);

        // Build the registration URL
        var registerUrl = $"{publicUrl}/Account/Register/Register?returnUrl={fullReturnUrl}";

        return Redirect(registerUrl);
    }
}