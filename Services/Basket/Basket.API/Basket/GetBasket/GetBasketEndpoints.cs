using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Basket.API.Basket.GetBasket;

public record GetBasketResponse(ShoppingCart Cart);

public class GetBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/basket/{userName}",
            [Authorize] async (string userName, ISender sender, HttpContext context, ILogger<GetBasketEndpoints> logger) =>
            {
                // Get the authenticated user's name from claims
                var authenticatedUserName = context.User.FindFirst(ClaimTypes.Name)?.Value
                    ?? context.User.FindFirst("name")?.Value
                    ?? context.User.Identity?.Name;

                logger.LogInformation("GetBasket called for {UserName} by authenticated user {AuthUser}",
                    userName, authenticatedUserName);

                // Check if user is accessing their own basket or is admin
                if (authenticatedUserName != userName && !context.User.IsInRole("Admin"))
                {
                    logger.LogWarning("User {AuthUser} attempted to access basket of {UserName}",
                        authenticatedUserName, userName);
                    return Results.Forbid();
                }

                var result = await sender.Send(new GetBasketQuery(userName));
                var response = result.Adapt<GetBasketResponse>();
                return Results.Ok(response);
            })
        .WithName("GetBasket")
        .Produces<GetBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .WithSummary("Get Basket")
        .WithDescription("Get Basket")
        .RequireAuthorization();
    }
}