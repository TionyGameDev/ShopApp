using FluentAssertions;
using Moq;
using ShopApp.Application.DTOs.Products;
using ShopApp.Application.Interfaces;
using ShopApp.Application.Services.ProductServices;
using ShopApp.Domain.Entites;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Tests.TestServies;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _productService = new ProductService(_productRepositoryMock.Object, _cacheServiceMock.Object);
    }

    [Fact]
    private async Task GetProductById_ExistingProduct_ReturnsProductDto()
    {
        Guid productId = Guid.NewGuid();
        
         _cacheServiceMock.Setup(c => c.GetAsync<Product>(productId.ToString()))
            .ReturnsAsync((Product?)null);
        
        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(new Product{Id = productId,Name = "Яблоко"});
        
        var result = await _productService.GetProductById(productId);
        
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be("Яблоко");   
    }
    
    [Fact]
    private async Task GetProductById_ExistingProduct_ReturnsNotFound()
    {
        Guid productId = Guid.NewGuid();
        
        _cacheServiceMock.Setup(c => c.GetAsync<Product>(productId.ToString()))
            .ReturnsAsync((Product?)null);
        
        _productRepositoryMock.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);
        
        var result = await Assert.ThrowsAsync<NotFoundException>
            (async () => await _productService.GetProductById(productId));

        result.Message.Should().Contain("not found");
    }
    
    [Fact]
    private async Task CreateProduct_ValidDto_ReturnsProductDto()
    {
        var productDto = new CreateProductDto("Яблоко","Без описания", 10, 50);

        //_cacheServiceMock.Setup(c => c.GetAsync<Product>(productId.ToString())).ReturnsAsync((Product?)null);
        
        _productRepositoryMock.Setup(d => d.AddAsync(It.IsAny<Product>())).ReturnsAsync(new Product
        {
            Name = productDto.Name,
            Price = productDto.Price,
            Stock = productDto.Stock,
            Description = productDto.Description
        });

        var result = await _productService.CreateProduct(productDto);
        
        result.Should().NotBeNull();
        result.Name.Should().Be("Яблоко");
        result.Price.Should().Be(10);
    }
}