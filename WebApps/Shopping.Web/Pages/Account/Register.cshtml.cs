using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        // Используем PublicAuthority для внешних редиректов
        var publicAuthority = _configuration["IdentityServer:PublicAuthority"]
            ?? _configuration["IdentityServer:Authority"];

        return Redirect($"{publicAuthority}/Account/Register?returnUrl={Uri.EscapeDataString($"{Request.Scheme}://{Request.Host}/signin-oidc")}");
    }
}