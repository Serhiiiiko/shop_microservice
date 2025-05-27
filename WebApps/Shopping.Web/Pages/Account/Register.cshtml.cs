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
        // Redirect directly to Identity Server's registration page
        var identityServerUrl = _configuration["IdentityServer:Authority"];
        var publicUrl = _configuration["IdentityServer:PublicAuthority"] ?? identityServerUrl;

        var registerUrl = $"{publicUrl}/Account/Register/Register?returnUrl={Uri.EscapeDataString(returnUrl)}";

        return Redirect(registerUrl);
    }
}