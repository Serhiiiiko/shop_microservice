using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Shopping.Web.Pages
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class CheckoutModel
        (IBasketService basketService, ILogger<CheckoutModel> logger)
        : PageModel
    {
        [BindProperty]
        public BasketCheckoutModel Order { get; set; } = default!;
        public ShoppingCartModel Cart { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            Cart = await basketService.LoadUserBasket(User);
            return Page();
        }

        public async Task<IActionResult> OnPostCheckOutAsync()
        {
            logger.LogInformation("Checkout button clicked");

            Cart = await basketService.LoadUserBasket(User);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Получаем ID пользователя из claim "sub" (subject)
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                logger.LogError("User ID claim not found");
                return Page();
            }

            var userId = userIdClaim.Value;
            logger.LogInformation("Checkout for user ID: {UserId}", userId);

            // Проверяем, является ли userId валидным Guid
            if (!Guid.TryParse(userId, out var customerId))
            {
                logger.LogError("Invalid user ID format: {UserId}", userId);
                ModelState.AddModelError("", "Invalid user ID format");
                return Page();
            }

            Order.CustomerId = customerId;
            Order.UserName = Cart.UserName;
            Order.TotalPrice = Cart.TotalPrice;

            await basketService.CheckoutBasket(new CheckoutBasketRequest(Order));

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}