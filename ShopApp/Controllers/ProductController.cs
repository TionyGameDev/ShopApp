using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.CQRS.Products.Commands;
using ShopApp.Application.CQRS.Products.Queries;
using ShopApp.Application.DTOs.Products;
using ShopApp.Application.Services.ProductServices;

namespace ShopApp.Controllers;

[ApiController]
[Route("api/product")]
public class ProductController(IMediator mediator) : ControllerBase
{
    [HttpPut] [Authorize(Roles = "Admin")]
    [EndpointSummary("Обновить товар")]
    public async Task<ActionResult> UpdateProduct([FromBody] ProductDto product)
    {
        await mediator.Send(new UpdateProductCommand(product));
        return Ok();
    }
    [HttpDelete("{id:guid}")] [Authorize(Roles = "Admin")]
    [EndpointSummary("Убрать товар")]
    public async Task<ActionResult> RemoveProduct(Guid id)
    {
        await mediator.Send(new DeleteProductCommand(id));
        return Ok();
    }
    
    [HttpPost] [Authorize(Roles = "Admin")]
    [EndpointSummary("Создать товар")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto product)
    {
        var productNew = await mediator.Send(new CreateProductCommand(product));
        return CreatedAtAction(nameof(GetProductById), new {id = productNew.Id}, product);
    }
    
    [HttpGet] [Authorize]
    [EndpointSummary("Все товары")]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        var result = await mediator.Send(new GetProductsQuery());
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]  [Authorize]
    [EndpointSummary("Взять товар по ID")]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return Ok(result);
        
    }
    
}