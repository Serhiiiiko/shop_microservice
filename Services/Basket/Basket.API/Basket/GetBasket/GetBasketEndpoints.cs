using Microsoft.AspNetCore.Authorization;

namespace Basket.API.Basket.GetBasket;

public record GetBasketResponse(ShoppingCart Cart);

public class GetBasketEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/basket/{userName}",
            [Authorize] async (string userName, ISender sender, HttpContext context) =>
            {
                var currentUser = context.User.Identity?.Name;
                if (currentUser != userName && !context.User.IsInRole("Admin"))
                {
                    return Results.Forbid();
                }

                var result = await sender.Send(new GetBasketQuery(userName));
                var response = result.Adapt<GetBasketResponse>();
                return Results.Ok(response);
            })
        .WithName("GetProductById")
        .Produces<GetBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Get Product By Id")
        .WithDescription("Get Product By Id")
        .RequireAuthorization();
    }
}