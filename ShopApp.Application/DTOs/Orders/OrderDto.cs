using ShopApp.Domain.Entites;

namespace ShopApp.Application.DTOs.Orders;

public record OrderDto(Guid OrderId,Guid UserId,List<OrderItemDto> Items,StatusOrder OrderStatus,DateTime DateCreated);

  
