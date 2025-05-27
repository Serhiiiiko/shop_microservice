using Microsoft.AspNetCore.Authorization;

namespace Basket.API.Basket.StoreBasket;

public record StoreBasketRequest(ShoppingCart Cart);
public record StoreBasketResponse(string UserName);

public class StoreBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket",
            [Authorize] async (StoreBasketRequest request, ISender sender, HttpContext context) =>
            {
                var currentUser = context.User.Identity?.Name;
                if (currentUser != request.Cart.UserName && !context.User.IsInRole("Admin"))
                {
                    return Results.Forbid();
                }

                var command = request.Adapt<StoreBasketCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<StoreBasketResponse>();

                return Results.Created($"/basket/{response.UserName}", response);
            })
        .WithName("CreateProduct")
        .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Product")
        .WithDescription("Create Product")
        .RequireAuthorization();
    }
}