using ShopApp.Application.CQRS.Products.Commands;
using ShopApp.Application.DTOs.Products;
using ShopApp.Domain.Entites;

namespace ShopApp.Application.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(this Product p)
        => new(p.Id, p.Name, p.Description, p.Price, p.Stock); 
    public static Product NewProduct(this CreateProductDto p)
    {
        var product = new Product();
        product.Name = p.Name;
        product.Description = p.Description;
        product.Price = p.Price;
        product.Stock = p.Stock;
        return product;
    }
    
    
}