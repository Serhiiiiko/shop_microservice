using Microsoft.AspNetCore.Authorization;

namespace Shopping.Web.Pages;
public class IndexModel
    (ICatalogService catalogService, IBasketService basketService, ILogger<IndexModel> logger)
    : PageModel
{
    public IEnumerable<ProductModel> ProductList { get; set; } = new List<ProductModel>();

    public async Task<IActionResult> OnGetAsync()
    {
        logger.LogInformation("Index page visited");
        var result = await catalogService.GetProducts();
        ProductList = result.Products;
        return Page();
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> OnPostAddToCartAsync(Guid productId)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            return RedirectToPage("/Account/Login", new { returnUrl = Request.Path });
        }

        logger.LogInformation("Add to cart button clicked");

        var productResponse = await catalogService.GetProduct(productId);
        var basket = await basketService.LoadUserBasket(User);

        basket.Items.Add(new ShoppingCartItemModel
        {
            ProductId = productId,
            ProductName = productResponse.Product.Name,
            Price = productResponse.Product.Price,
            Quantity = 1,
            Color = "Black"
        });

        await basketService.StoreBasket(new StoreBasketRequest(basket));

        return RedirectToPage("Cart");
    }
}