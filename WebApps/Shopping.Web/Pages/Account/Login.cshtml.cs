using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Shopping.Web.Pages.Account;

public class LoginModel : PageModel
{
    public async Task<IActionResult> OnGetAsync(string returnUrl = "/")
    {
        // Ensure we have a proper return URL
        if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/Account/Login")
        {
            returnUrl = "/";
        }

        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl
        };

        return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
    }
}