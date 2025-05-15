namespace Shopping.Web.Services;

public interface ICatalogService
{
    [Get("/catalog-service/products?pageNumber={pageNumber}&pageSize={pageSize}")]
    Task<GerProductsResponse> GetProducts(int? pageNumber = 1, int? pageSize = 10);

    [Get("/catalog-service/products/category/{category}")]
    Task<GerProductsByCategoryResponse> GetProductsByCategory (string category);

    [Get("/catalog-service/products/{id}")]
    Task<GerProductsByIdResponse> GetProduct(Guid id);
}
