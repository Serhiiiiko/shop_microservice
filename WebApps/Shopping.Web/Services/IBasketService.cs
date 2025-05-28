using System.Net;
using System.Security.Claims;

namespace Shopping.Web.Services;

public interface IBasketService
{
    [Get("/basket-service/basket/{userName}")]
    Task<GetBasketResponse> GetBasket(string userName);

    [Post("/basket-service/basket")]
    Task<StoreBasketResponse> StoreBasket(StoreBasketRequest request);

    [Delete("/basket-service/basket/{userName}")]
    Task<DeleteBasketResponse> DeleteBasket(string userName);

    [Post("/basket-service/basket/checkout")]
    Task<CheckoutBasketResponse> CheckoutBasket(CheckoutBasketRequest request);

    public async Task<ShoppingCartModel> LoadUserBasket(ClaimsPrincipal user)
    {
        var userName = user.Identity?.Name;

        if (string.IsNullOrEmpty(userName))
        {
            return new ShoppingCartModel
            {
                UserName = "Guest",
                Items = []
            };
        }

        ShoppingCartModel basket;

        try
        {
            var getBasketResponse = await GetBasket(userName);
            basket = getBasketResponse.Cart;
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.NotFound)
        {
            // Basket not found - create empty basket
            basket = new ShoppingCartModel
            {
                UserName = userName,
                Items = []
            };
        }
        catch (ApiException apiException) when (apiException.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Authentication issue - return empty basket
            basket = new ShoppingCartModel
            {
                UserName = userName,
                Items = []
            };
        }
        catch (Exception)
        {
            // Any other error - return empty basket
            basket = new ShoppingCartModel
            {
                UserName = userName ?? "Guest",
                Items = []
            };
        }

        return basket;
    }
}