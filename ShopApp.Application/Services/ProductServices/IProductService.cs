using ShopApp.Application.DTOs.Products;
using ShopApp.Domain.Entites;

namespace ShopApp.Application.Services.ProductServices;

public interface IProductService
{
  Task<ProductDto> CreateProduct(CreateProductDto product);
  Task RemoveProduct(Guid idProduct);
  Task<ProductDto> UpdateProduct(ProductDto product);
  Task<List<ProductDto>> GetProducts();
  Task<ProductDto> GetProductById(Guid idProduct);
  
  Task DecreaseStock (Guid idProduct,int amount);

}