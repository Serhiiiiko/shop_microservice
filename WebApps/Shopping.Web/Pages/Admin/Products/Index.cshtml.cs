// Create a new folder structure: WebApps/Shopping.Web/Pages/Admin/Products/

// WebApps/Shopping.Web/Pages/Admin/Products/Index.cshtml.cs
using Microsoft.AspNetCore.Authorization;

namespace Shopping.Web.Pages.Admin.Products
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ICatalogService catalogService, ILogger<IndexModel> logger)
        {
            _catalogService = catalogService;
            _logger = logger;
        }

        public IEnumerable<ProductModel> Products { get; set; } = new List<ProductModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _catalogService.GetProducts();
            Products = result.Products;
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _catalogService.DeleteProduct(id);
            return RedirectToPage();
        }
    }
}