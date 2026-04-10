using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.DTOs.Products;
using ShopApp.Application.Services.ProductServices;

namespace ShopApp.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController(IProductService service) : ControllerBase
{
    [HttpPut] [Authorize(Roles = "Admin")]
    [EndpointSummary("Обновить товар")]
    public async Task<ActionResult> UpdateProduct([FromBody] ProductDto product)
    {
         await service.UpdateProduct(product);
         return Ok();
    }
    [HttpDelete("{id:guid}")] [Authorize(Roles = "Admin")]
    [EndpointSummary("Убрать товар")]
    public async Task<ActionResult> RemoveProduct(Guid id)
    {
        await service.RemoveProduct(id);
        return Ok();
    }
    
    [HttpPost] [Authorize(Roles = "Admin")]
    [EndpointSummary("Создать товар")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto product)
    {
        var productNew = await service.CreateProduct(product);
        return CreatedAtAction(nameof(GetProductById), new {id = productNew.Id}, product);
    }
    
    [HttpGet] [Authorize]
    [EndpointSummary("Все товары")]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        var products = await service.GetProducts();
        return Ok(products);
    }
    
    [HttpGet("{id:guid}")]  [Authorize]
    [EndpointSummary("Взять товар по ID")]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        var products = await service.GetProductById(id);
        return Ok(products);
        
    }
    
}