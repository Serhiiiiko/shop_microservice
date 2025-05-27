using Microsoft.AspNetCore.Authorization;

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
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var customerId = Guid.Parse(userId ?? Guid.NewGuid().ToString());

            var response = await orderingService.GetOrdersByCustomer(customerId);
            Orders = response.Orders;

            return Page();
        }
    }
}