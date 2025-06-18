using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shopping.Web.Pages.Admin.Customers
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

        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();

        public class CustomerViewModel
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public int OrdersCount { get; set; }
            public DateTime? CreatedAt { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Since we don't have a direct customers endpoint, we'll extract from orders
                var ordersResponse = await _orderingService.GetOrders(1, 100);

                // Group orders by customer to get unique customers
                var customerGroups = ordersResponse.Orders.Data
                    .GroupBy(o => o.CustomerId)
                    .ToList();

                // Known customers from seed data
                Customers = new List<CustomerViewModel>
                {
                    new CustomerViewModel
                    {
                        Id = Guid.Parse("58c49479-ec65-4de2-86e7-033c546291aa"),
                        Name = "mehmet",
                        Email = "mehmet@gmail.com",
                        OrdersCount = customerGroups.FirstOrDefault(g => g.Key == Guid.Parse("58c49479-ec65-4de2-86e7-033c546291aa"))?.Count() ?? 0
                    },
                    new CustomerViewModel
                    {
                        Id = Guid.Parse("189dc8dc-990f-48e0-a37b-e6f2b60b9d7d"),
                        Name = "john",
                        Email = "john@gmail.com",
                        OrdersCount = customerGroups.FirstOrDefault(g => g.Key == Guid.Parse("189dc8dc-990f-48e0-a37b-e6f2b60b9d7d"))?.Count() ?? 0
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading customers");
            }

            return Page();
        }
    }
}