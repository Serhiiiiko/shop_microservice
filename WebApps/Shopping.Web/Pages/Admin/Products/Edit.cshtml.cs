using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shopping.Web.Services;
using System.ComponentModel.DataAnnotations;
using static Shopping.Web.Services.ICatalogService;
using Microsoft.AspNetCore.Authorization;

namespace Shopping.Web.Pages.Admin.Products
{
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ICatalogService _catalogService;
        private readonly ILogger<EditModel> _logger;

        public EditModel(ICatalogService catalogService, ILogger<EditModel> logger)
        {
            _catalogService = catalogService;
            _logger = logger;
        }

        [BindProperty]
        public EditProductViewModel Product { get; set; } = new();

        public class EditProductViewModel
        {
            public Guid Id { get; set; }

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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var response = await _catalogService.GetProduct(id);
            var product = response.Product;

            Product = new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = string.Join(", ", product.Category),
                ImageFilePath = product.ImageFilePath
            };

            return Page();
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

            var updateRequest = new UpdateProductRequest(
                Product.Id,
                Product.Price,
                Product.Name,
                Product.Description,
                categories,
                Product.ImageFilePath
            );

            await _catalogService.UpdateProduct(updateRequest);

            return RedirectToPage("Index");
        }
    }
}