namespace Ordering.Application.Orders.EventHandlers.Domain;
public class OrderUpdatedEventHandler
    : INotificationHandler<OrderCreatedEvent>
{
    public Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        
        return Task.CompletedTask;
    }
}
