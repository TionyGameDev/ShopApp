using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.DTOs.Orders;
using ShopApp.Application.Services.OrderServices;

namespace ShopApp.Controllers;

[ApiController]
[Route("api/order")]
public class OrdersController(IOrderService service) : ControllerBase
{
    [HttpGet("{id:guid}",Name = nameof(GetOrder))] [Authorize]
    [EndpointSummary("Взять ордер по ID")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        var order = await service.GetOrder(id);
        return Ok(order);
    }

    [HttpGet("user", Name = nameof(GetByUserId))] [Authorize]
    [EndpointSummary("Все ордера у пользователя")]
    public async Task<ActionResult<List<OrderDto>>> GetByUserId()
    {
        var id = GetId();
        var order = await service.GetByUserId(id);
        return Ok(order);
    }

    [HttpPost] [Authorize]
    [EndpointSummary("Создать ордер")]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto login)
    {
        var id = GetId();
        var order = await service.CreateOrder(login, id);
        return Ok(order);
    }

    [HttpPut("{id:guid}/cancel")] [Authorize]
    [EndpointSummary("Отменить ордер")]
    public async Task<ActionResult> CancelOrder(Guid id)
    {
        await service.CancelOrder(id);
        return Ok();
    }

    private Guid GetId() => Guid.Parse(User.FindFirst("id")!.Value);
}