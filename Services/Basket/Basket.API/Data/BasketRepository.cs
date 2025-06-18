namespace Basket.API.Data;

public class BasketRepository(IDocumentSession session, ILogger<BasketRepository> logger)
    : IBasketRepository
{
    public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting basket for user: {UserName}", userName);

        var basket = await session.LoadAsync<ShoppingCart>(userName, cancellationToken);

        if (basket is null)
        {
            logger.LogInformation("Basket not found for user: {UserName}, returning empty basket", userName);
            return new ShoppingCart(userName);
        }

        return basket;
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Storing basket for user: {UserName}", basket.UserName);

        session.Store(basket);
        await session.SaveChangesAsync(cancellationToken);
        return basket;
    }

    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting basket for user: {UserName}", userName);

        session.Delete<ShoppingCart>(userName);
        await session.SaveChangesAsync(cancellationToken);
        return true;
    }
}