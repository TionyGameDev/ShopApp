using ShopApp.Application.DTOs.Products;
using ShopApp.Application.Interfaces;
using ShopApp.Domain.Entites;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Services.ProductServices;

public class ProductService(IProductRepository productRepository, ICacheService cacheService) : IProductService
{
  public async Task<ProductDto> CreateProduct(CreateProductDto createProduct)
  {
    var product =  await productRepository.AddAsync(ConvertToProduct(createProduct));
    await cacheService.RemoveAsync("products");
    await cacheService.SetAsync(product.Id.ToString(),product);
    return ConvertToProductDto(product);
  }

  public async Task RemoveProduct(Guid idProduct)
  {
    var product = await productRepository.GetByIdAsync(idProduct);
    if (product != null)
    {
      await productRepository.RemoveAsync(product.Id);
      await cacheService.RemoveAsync("products");
      await cacheService.RemoveAsync(product.Id.ToString());
    }
    else
      throw new NotFoundException("Product", idProduct);
  }

  public async Task<ProductDto> UpdateProduct(ProductDto product)
  {
    var productToUpdate = await productRepository.GetByIdAsync(product.Id);
    if (productToUpdate != null)
    {
      productToUpdate.Name = product.Name;
      productToUpdate.Price = product.Price;
      productToUpdate.Stock = product.Stock;
      productToUpdate.Description = product.Description;
      
      await productRepository.UpdateAsync(productToUpdate);
      await cacheService.SetAsync(product.Id.ToString(),productToUpdate);
      
      return ConvertToProductDto(productToUpdate);
    }
    
    throw new NotFoundException("Product", product.Id);
  }

  public async Task<List<ProductDto>> GetProducts()
  {
    var products = await cacheService.GetAsync<List<Product>>("products");
    if (products == null)
    {
      products = await productRepository.GetAllAsync();
      await cacheService.SetAsync("products", products, TimeSpan.FromDays(1));
    }
    return products.Select(ConvertToProductDto).ToList();
  }

  public async Task<ProductDto> GetProductById(Guid idProduct)
  {
    var product = await cacheService.GetAsync<Product>(idProduct.ToString());
    if (product == null)
    {
      product = await productRepository.GetByIdAsync(idProduct) ?? throw new NotFoundException("Product", idProduct);
      await cacheService.SetAsync(idProduct.ToString(), product, TimeSpan.FromDays(1));
    }
    return ConvertToProductDto(product);
  }

  public async Task DecreaseStock(Guid idProduct,int amount)
  {
    var product = await productRepository.GetByIdAsync(idProduct);
    if (product != null)
    {
      product.DecreaseStock(amount);
      await productRepository.UpdateAsync(product);
    }
  }

  private Product ConvertToProduct(CreateProductDto productDto)
  {
    var product = new Product();
    product.Name = productDto.Name;
    product.Price = productDto.Price;
    product.Stock = productDto.Stock;
    product.Description = productDto.Description;

    return product;
  }
  
  private ProductDto ConvertToProductDto(Product product)
  {
    var productDto = new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock);
    return productDto;
  }
}