using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Shopping.Web.Pages
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class OrderListModel
        (IOrderingService orderingService, ILogger<OrderListModel> logger)
        : PageModel
    {
        public IEnumerable<OrderModel> Orders { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Получаем ID пользователя из claim "sub" (subject)
            var userIdClaim = User.FindFirst("sub") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                logger.LogError("User ID claim not found");
                Orders = new List<OrderModel>();
                return Page();
            }

            var userId = userIdClaim.Value;
            var customerId = Guid.Parse(userId);

            logger.LogInformation("Getting orders for customer ID: {CustomerId}", customerId);

            try
            {
                var response = await orderingService.GetOrdersByCustomer(customerId);
                Orders = response.Orders;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting orders for customer {CustomerId}", customerId);
                Orders = new List<OrderModel>();
            }

            return Page();
        }
    }
}