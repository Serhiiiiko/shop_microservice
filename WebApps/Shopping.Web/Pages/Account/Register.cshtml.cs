using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shopping.Web.Pages.Account;

public class RegisterModel : PageModel
{
    public IActionResult OnGet(string returnUrl = "/")
    {
        return Redirect($"{Configuration["IdentityServer:Authority"]}/Account/Register?returnUrl={Uri.EscapeDataString($"{Request.Scheme}://{Request.Host}/signin-oidc")}");
    }

    private IConfiguration Configuration => HttpContext.RequestServices.GetRequiredService<IConfiguration>();
}