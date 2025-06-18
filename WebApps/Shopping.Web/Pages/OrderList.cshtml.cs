using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Shopping.Web.Pages
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class OrderListModel(IOrderingService orderingService, IBasketService basketService, ILogger<OrderListModel> logger)
        : PageModel
    {
        public IEnumerable<OrderModel> Orders { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get user's email from claims
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value
                    ?? User.FindFirst("email")?.Value;

                var userName = User.Identity?.Name
                    ?? User.FindFirst(ClaimTypes.Name)?.Value
                    ?? User.FindFirst("name")?.Value;

                if (string.IsNullOrEmpty(userEmail) && !string.IsNullOrEmpty(userName))
                {
                    // If no email in claims, try to get orders by name
                    logger.LogInformation("No email found in claims, searching orders by name: {UserName}", userName);
                    var ordersByName = await orderingService.GetOrdersByName(userName);
                    Orders = ordersByName.Orders;
                }
                else if (!string.IsNullOrEmpty(userEmail))
                {
                    // Get orders by email through the order name (which contains username)
                    // In a real app, we'd have a GetOrdersByEmail endpoint
                    logger.LogInformation("Searching orders for user: {UserName} with email: {Email}", userName, userEmail);

                    if (!string.IsNullOrEmpty(userName))
                    {
                        var ordersByName = await orderingService.GetOrdersByName(userName);
                        Orders = ordersByName.Orders;
                    }
                }
                else
                {
                    logger.LogWarning("No user identification found in claims");
                    Orders = [];
                }

                logger.LogInformation("Loaded {Count} orders for user", Orders.Count());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading orders");
                Orders = [];
            }

            return Page();
        }
    }
}