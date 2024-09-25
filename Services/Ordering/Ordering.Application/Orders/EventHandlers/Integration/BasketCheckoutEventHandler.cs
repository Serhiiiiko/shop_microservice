using BuildingBlockMessanging.Events;
using MassTransit;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.Application.Orders.EventHandlers.Integration;
public class BasketCheckoutEventHandler(ISender sender, ILogger<BasketCheckoutEventHandler> logger)
    : IConsumer<BasketCheckoutEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        logger.LogInformation("Integration Event Handled: {IntegrationEventId}", context.Message.GetType().Name);

        var command = MapToCreateOrderCommand(context.Message);

        await sender.Send(command);
    }

    private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutEvent message)
    {
        var adressDto = new AddressDto(message.FirstName, message.LastName, message.EmailAddress, message.AddressLine, message.Country, message.State, message.ZipCode);;
        var paymentDto = new PaymentDto(message.CardName, message.CardNumber, message.Expiration, message.CVV, message.PaymentMethod);
        var orderId = Guid.NewGuid();

        var orderDto = new OrderDto(
            Id: orderId,
            CustomerId : message.CustomerId,
            OrderName : message.UserName,
            ShippingAddress : adressDto,
            BillingAddress : adressDto,
            Payment : paymentDto,
            Status : Ordering.Domain.Enums.OrderStatus.Pending,
            OrderItems:
            [ 
              new OrderItemDto(orderId, Guid.NewGuid(), 2, 500),
              new OrderItemDto(orderId, Guid.NewGuid(), 1, 1000)
            ]);

        return new CreateOrderCommand(orderDto);
    }
}
