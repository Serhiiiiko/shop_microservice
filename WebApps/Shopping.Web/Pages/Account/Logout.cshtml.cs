using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Shopping.Web.Pages.Account;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnPostAsync(string returnUrl = "/")
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

        return LocalRedirect(returnUrl);
    }
}