namespace ShopApp.Application.DTOs.Products;

public record CreateProductDto(string Name, string Description, decimal Price, int Stock);
