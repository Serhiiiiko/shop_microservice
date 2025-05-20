// Update WebApps/Shopping.Web/Services/ICatalogService.cs
using Refit;
using Shopping.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shopping.Web.Services
{
    public interface ICatalogService
    {
        // Existing methods
        [Get("/catalog-service/products")]
        Task<GerProductsResponse> GetProducts(int? pageNumber = 1, int? pageSize = 10);

        [Get("/catalog-service/products/category/{category}")]
        Task<GerProductsByCategoryResponse> GetProductsByCategory(string category);

        [Get("/catalog-service/products/{id}")]
        Task<GerProductsByIdResponse> GetProduct([AliasAs("id")] Guid id);

        // Add these new CRUD operations
        [Post("/catalog-service/products")]
        Task<CreateProductResponse> CreateProduct([Body] CreateProductRequest request);

        [Put("/catalog-service/products")]
        Task<UpdateProductResponse> UpdateProduct([Body] UpdateProductRequest request);

        [Delete("/catalog-service/products/{id}")]
        Task<DeleteProductResponse> DeleteProduct([AliasAs("id")] Guid id);
    }

    // Define these model classes right here for simplicity
    public record CreateProductRequest(decimal Price, string Name, string Description, List<string> Category, string ImageFilePath);
    public record CreateProductResponse(Guid Id);

    public record UpdateProductRequest(Guid Id, decimal Price, string Name, string Description, List<string> Category, string ImageFilePath);
    public record UpdateProductResponse(bool IsSuccess);

    public record DeleteProductResponse(bool IsSuccess);
}