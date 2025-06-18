namespace Basket.API.Basket.GetBasket;

public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;

public record GetBasketResult(ShoppingCart Cart);

public class GetBasketQueryHandler(IBasketRepository repository, ILogger<GetBasketQueryHandler> logger)
    : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var basket = await repository.GetBasket(query.UserName, cancellationToken);
            return new GetBasketResult(basket);
        }
        catch (BasketNotFoundException)
        {
            // If basket doesn't exist, create an empty one
            logger.LogInformation("Basket not found for user {UserName}, creating empty basket", query.UserName);

            var newBasket = new ShoppingCart(query.UserName);
            return new GetBasketResult(newBasket);
        }
    }
}