using ShopApp.Application.DTOs.Products;
using ShopApp.Application.Interfaces;
using ShopApp.Domain.Entites;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Services.ProductServices;

public class ProductService : IProductService
{
  private readonly IProductRepository _productRepository;

  public ProductService(IProductRepository productRepository) => _productRepository = productRepository;

  public async Task<ProductDto> CreateProduct(CreateProductDto createProduct)
  {
    var product =  await _productRepository.AddAsync(ConvertToProduct(createProduct));
    return ConvertToProductDto(product);
  }

  public async Task RemoveProduct(Guid idProduct)
  {
    var product = await _productRepository.GetByIdAsync(idProduct);
    if (product != null)
      await _productRepository.RemoveAsync(product.Id);
    else
      throw new NotFoundException("Product", idProduct);
  }

  public async Task<ProductDto> UpdateProduct(ProductDto product)
  {
    var productToUpdate = await _productRepository.GetByIdAsync(product.Id);
    if (productToUpdate != null)
    {
      productToUpdate.Name = product.Name;
      productToUpdate.Price = product.Price;
      productToUpdate.Stock = product.Stock;
      productToUpdate.Description = product.Description;
      
      await _productRepository.UpdateAsync(productToUpdate);
      return ConvertToProductDto(productToUpdate);
    }
    
    throw new NotFoundException("Product", product.Id);
  }

  public async Task<List<ProductDto>> GetProducts()
  {
    var list =  await _productRepository.GetAllAsync();
    return list.Select(ConvertToProductDto).ToList();
  }

  public async Task<ProductDto> GetProductById(Guid idProduct)
  {
    var product = await _productRepository.GetByIdAsync(idProduct);
    if (product != null)
      return ConvertToProductDto(product);
    
    throw new NotFoundException("Product", idProduct);
  }

  public async Task DecreaseStock(Guid idProduct,int amount)
  {
    var product = await _productRepository.GetByIdAsync(idProduct);
    if (product != null)
    {
      product.DecreaseStock(amount);
      await _productRepository.UpdateAsync(product);
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