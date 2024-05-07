namespace Catalog.API.Products.GetProductById;

public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdRusult>;

public record GetProductByIdRusult(Product Product);

internal class GetProductByIdQueryHandler
    (IDocumentSession session)
    : IQueryHandler<GetProductByIdQuery, GetProductByIdRusult>
{
    public async Task<GetProductByIdRusult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await session.LoadAsync<Product>(query.Id, cancellationToken);

        return product is null ? throw new ProductNotFoundException(query.Id) : new GetProductByIdRusult(product);
    }
}

