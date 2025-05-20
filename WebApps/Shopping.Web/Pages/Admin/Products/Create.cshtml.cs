// Update WebApps/Shopping.Web/Pages/Admin/Products/Create.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Services;
using System.ComponentModel.DataAnnotations;
using static Shopping.Web.Services.ICatalogService;

namespace Shopping.Web.Pages.Admin.Products
{
    public class CreateModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ICatalogService catalogService, ILogger<CreateModel> logger)
        {
            _catalogService = catalogService;
            _logger = logger;
        }

        [BindProperty]
        public CreateProductViewModel Product { get; set; } = new();

        public class CreateProductViewModel
        {
            [Required]
            public string Name { get; set; } = string.Empty;

            [Required]
            public string Description { get; set; } = string.Empty;

            [Required]
            [Range(0.01, 10000)]
            public decimal Price { get; set; }

            [Required]
            public string Category { get; set; } = string.Empty;

            public string ImageFilePath { get; set; } = "product-1.png";
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Convert comma-separated categories to List<string>
            var categories = Product.Category.Split(',')
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrEmpty(c))
                .ToList();

            var createRequest = new CreateProductRequest (
                Product.Price,
                Product.Name,
                Product.Description,
                categories,
                Product.ImageFilePath
            );

            await _catalogService.CreateProduct(createRequest);

            return RedirectToPage("Index");
        }
    }
}