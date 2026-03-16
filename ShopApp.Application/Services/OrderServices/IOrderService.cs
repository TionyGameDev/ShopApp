using ShopApp.Application.DTOs.Orders;

namespace ShopApp.Application.Services.OrderServices;

public interface IOrderService
{
  Task<OrderDto> CreateOrder(CreateOrderDto order, Guid userId);
  Task<OrderDto> GetOrder(Guid idOrder);
  Task<List<OrderDto>> GetByUserId(Guid userId);
  Task CancelOrder(Guid idOrder);
}