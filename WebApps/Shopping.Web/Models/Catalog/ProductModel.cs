// WebApps/Shopping.Web/Models/Catalog/ProductModel.cs
namespace Shopping.Web.Models.Catalog;

public class ProductModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public string ImageFilePath { get; set; } = default!; // Изменено с ImageFile на ImageFilePath
    public List<string> Category { get; set; } = new();

    // Добавляем свойство для обратной совместимости
    public string ImageFile => ImageFilePath;
}

public record GerProductsResponse(IEnumerable<ProductModel> Products);
public record GerProductsByCategoryResponse(IEnumerable<ProductModel> Products);
public record GerProductsByIdResponse(ProductModel Product);