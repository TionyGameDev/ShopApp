using ShopApp.Application.DTOs.Products;

namespace ShopApp.Application.DTOs.Orders;

public record OrderItemDto(Guid ProductId, string ProductName, int Quantity, decimal Price);


  
