using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Models.Ordering;

namespace Shopping.Web.Pages.Admin.Orders
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

        public IEnumerable<OrderModel> Orders { get; set; } = new List<OrderModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                var response = await _orderingService.GetOrders(1, 100);
                Orders = response.Orders.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading orders");
            }

            return Page();
        }
    }
}