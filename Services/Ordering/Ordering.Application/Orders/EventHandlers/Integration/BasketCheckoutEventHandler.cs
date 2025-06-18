using BuildingBlockMessanging.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Orders.Commands.CreateOrder;

namespace Ordering.Application.Orders.EventHandlers.Integration;
public class BasketCheckoutEventHandler(ISender sender, ILogger<BasketCheckoutEventHandler> logger, IApplicationDbContext dbContext)
    : IConsumer<BasketCheckoutEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        logger.LogInformation("Integration Event Handled: {IntegrationEventId}", context.Message.GetType().Name);

        try
        {
            // Get or create customer based on email
            var customerId = await GetOrCreateCustomerByEmail(context.Message);

            var command = await MapToCreateOrderCommand(context.Message, customerId);
            await sender.Send(command);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling BasketCheckoutEvent for user {UserName}", context.Message.UserName);
            throw;
        }
    }

    private async Task<Guid> GetOrCreateCustomerByEmail(BasketCheckoutEvent message)
    {
        // First try to find customer by email
        var existingCustomer = await dbContext.Customers
            .Where(c => c.Email == message.EmailAddress)
            .FirstOrDefaultAsync();

        if (existingCustomer != null)
        {
            logger.LogInformation("Found existing customer with email {Email}", message.EmailAddress);
            return existingCustomer.Id.Value;
        }

        // Create new customer if not exists
        var newCustomerId = Guid.NewGuid();
        var customer = Customer.Create(
            CustomerId.Of(newCustomerId),
            message.UserName,
            message.EmailAddress
        );

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(default);

        logger.LogInformation("Created new customer {CustomerId} with email {Email}",
            newCustomerId, message.EmailAddress);

        return newCustomerId;
    }

    private async Task<List<(Guid productId, int quantity, decimal price)>> EnsureProductsExist()
    {
        // Hardcoded product IDs from initial data
        var productData = new List<(Guid id, string name, decimal price)>
        {
            (new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61"), "IPhone X", 500),
            (new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914"), "Samsung 10", 400)
        };

        foreach (var (id, name, price) in productData)
        {
            var productId = ProductId.Of(id);
            var existingProduct = await dbContext.Products.FindAsync(productId);

            if (existingProduct == null)
            {
                var product = Product.Create(productId, name, price);
                dbContext.Products.Add(product);
            }
        }

        await dbContext.SaveChangesAsync(default);

        // Return order items
        return new List<(Guid, int, decimal)>
        {
            (productData[0].id, 2, productData[0].price),
            (productData[1].id, 1, productData[1].price)
        };
    }

    private async Task<CreateOrderCommand> MapToCreateOrderCommand(BasketCheckoutEvent message, Guid customerId)
    {
        var addressDto = new AddressDto(message.FirstName, message.LastName, message.EmailAddress,
            message.AddressLine, message.Country, message.State, message.ZipCode);
        var paymentDto = new PaymentDto(message.CardName, message.CardNumber, message.Expiration,
            message.CVV, message.PaymentMethod);
        var orderId = Guid.NewGuid();

        // Ensure products exist and get order items
        var orderItems = await EnsureProductsExist();

        var orderDto = new OrderDto(
            Id: orderId,
            CustomerId: customerId, // Use the actual customer ID from database
            OrderName: message.UserName,
            ShippingAddress: addressDto,
            BillingAddress: addressDto,
            Payment: paymentDto,
            Status: Ordering.Domain.Enums.OrderStatus.Pending,
            OrderItems: orderItems.Select(item =>
                new OrderItemDto(orderId, item.productId, item.quantity, item.price)).ToList()
        );

        return new CreateOrderCommand(orderDto);
    }
}