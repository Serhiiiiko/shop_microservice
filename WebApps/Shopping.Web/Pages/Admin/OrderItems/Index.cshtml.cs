using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Models.Ordering;

namespace Shopping.Web.Pages.Admin.OrderItems
{
    public class IndexModel : PageModel
    {
        private readonly IOrderingService _orderingService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IOrderingService orderingService, ILogger<IndexModel> logger)
        {
            _orderingService = orderingService;
            _logger = logger;
        }

        public List<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get all orders and extract their items
                var response = await _orderingService.GetOrders(1, 100);
                OrderItems = response.Orders.Data
                    .SelectMany(o => o.OrderItems)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading order items");
            }

            return Page();
        }
    }
}