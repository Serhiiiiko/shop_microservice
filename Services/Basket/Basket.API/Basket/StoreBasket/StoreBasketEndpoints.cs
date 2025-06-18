using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Basket.API.Basket.StoreBasket;

public record StoreBasketRequest(ShoppingCart Cart);
public record StoreBasketResponse(string UserName);

public class StoreBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket",
            [Authorize] async (StoreBasketRequest request, ISender sender, HttpContext context, ILogger<StoreBasketEndpoints> logger) =>
            {
                // Get the authenticated user's name from claims
                var authenticatedUserName = context.User.FindFirst(ClaimTypes.Name)?.Value
                    ?? context.User.FindFirst("name")?.Value
                    ?? context.User.Identity?.Name;

                logger.LogInformation("StoreBasket called for basket user {BasketUser} by authenticated user {AuthUser}",
                    request.Cart.UserName, authenticatedUserName);

                // Check if user is storing their own basket or is admin
                if (authenticatedUserName != request.Cart.UserName && !context.User.IsInRole("Admin"))
                {
                    logger.LogWarning("User {AuthUser} attempted to store basket for {BasketUser}",
                        authenticatedUserName, request.Cart.UserName);
                    return Results.Forbid();
                }

                var command = request.Adapt<StoreBasketCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<StoreBasketResponse>();

                return Results.Created($"/basket/{response.UserName}", response);
            })
        .WithName("StoreBasket")
        .Produces<StoreBasketResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .WithSummary("Store Basket")
        .WithDescription("Store Basket")
        .RequireAuthorization();
    }
}