namespace Catalog.API.Products.GetProducts;

public record GetProductsQuery(int? PageNumber = 1, int? PageSize = 10): IQuery<GetProductsRusult>;

public record GetProductsRusult(IEnumerable<Product> Products);

internal class GetProductQueryHandler
    (IDocumentSession session)
    : IQueryHandler<GetProductsQuery, GetProductsRusult>
{
    public async Task<GetProductsRusult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await session.Query<Product>()
            .ToPagedListAsync(query.PageNumber ?? 1, query.PageSize ?? 10, cancellationToken);

        return new GetProductsRusult(products);
    }
}
