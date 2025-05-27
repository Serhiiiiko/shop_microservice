using Microsoft.AspNetCore.Authorization;

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

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Order.CustomerId = Guid.Parse(userId ?? Guid.NewGuid().ToString());
            Order.UserName = Cart.UserName;
            Order.TotalPrice = Cart.TotalPrice;

            await basketService.CheckoutBasket(new CheckoutBasketRequest(Order));

            return RedirectToPage("Confirmation", "OrderSubmitted");
        }
    }
}