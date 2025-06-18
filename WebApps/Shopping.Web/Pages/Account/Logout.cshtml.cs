using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Shopping.Web.Pages.Account;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnPostAsync(string returnUrl = "/")
    {
        // Sign out from both cookie and OpenID Connect schemes
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl
        };

        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, properties);

        return LocalRedirect(returnUrl);
    }
}