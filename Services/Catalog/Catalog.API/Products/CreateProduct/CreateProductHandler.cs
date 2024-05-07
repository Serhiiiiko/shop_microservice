namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(decimal Price, string Name, string Description, List<string> Category, string ImageFilePath)
    : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be higher");
        RuleFor(x=> x.Name).NotEmpty().WithMessage("Name Is required");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description Is required");
        RuleFor(x => x.Category).NotEmpty().WithMessage("Category Is required");
    }
}

internal class CreateProductCommandHandler
    (IDocumentSession session)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Price = command.Price,
            Name = command.Name,
            Description = command.Description,
            Category = command.Category,
            ImageFilePath = command.ImageFilePath,
        };

        session.Store(product);
        await session.SaveChangesAsync(cancellationToken);

        return new CreateProductResult(product.Id);
    }
}