using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Shopping.Web.Pages
{
    [Microsoft.AspNetCore.Authorization.Authorize] 
    public class CheckoutModel(IBasketService basketService, ILogger<CheckoutModel> logger)
        : PageModel
    {
        [BindProperty]
        public BasketCheckoutModel Order { get; set; } = default!;

        public ShoppingCartModel Cart { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            Cart = await basketService.LoadUserBasket(User);

            // Pre-fill form with user information from claims
            Order = new BasketCheckoutModel
            {
                UserName = User.Identity?.Name ?? User.FindFirst("name")?.Value ?? "",
                EmailAddress = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value ?? "",
                FirstName = User.FindFirst("given_name")?.Value ?? User.FindFirst(ClaimTypes.GivenName)?.Value ?? "",
                LastName = User.FindFirst("family_name")?.Value ?? User.FindFirst(ClaimTypes.Surname)?.Value ?? ""
            };

            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            logger.LogInformation("Checkout process started");

            Cart = await basketService.LoadUserBasket(User);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // ВАЖНО: Всегда используем имя из токена, а НЕ из формы
            Order.UserName = User.Identity?.Name ?? User.FindFirst("name")?.Value ?? "Guest";

            // Убедимся, что email тоже корректный
            if (string.IsNullOrEmpty(Order.EmailAddress))
            {
                Order.EmailAddress = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value ?? "";
            }

            // Generate a temporary CustomerId (will be replaced by actual customer ID in the handler)
            Order.CustomerId = Guid.Empty; // This will be ignored in the handler
            Order.TotalPrice = Cart.TotalPrice;

            logger.LogInformation("Processing checkout for user {UserName} with email {Email}",
                Order.UserName, Order.EmailAddress);

            await basketService.CheckoutBasket(new CheckoutBasketRequest(Order));

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}