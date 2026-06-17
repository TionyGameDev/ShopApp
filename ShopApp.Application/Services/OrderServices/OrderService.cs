using System.Transactions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.DTOs.Orders;
using ShopApp.Application.Interfaces;
using ShopApp.Application.Transations;
using ShopApp.Domain.Entites;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Services.OrderServices;

public class OrderService(IOrderRepository orderRepository, IProductRepository productRepository
  ,IEventBus eventBus, IUnitOfWork unitOfWork,IOrderNotificationService notification
  ,ILogger<OrderService> logger) : IOrderService
{
  public async Task<OrderDto> CreateOrder(CreateOrderDto createOrderDto, Guid userId)
  {
    logger.LogInformation("Создаём заказ для пользователя {UserId}, товар {ProductId}", 
      userId, createOrderDto.ProductId);
    
    await unitOfWork.BeginTransactionAsync(IsolationLevel.ReadCommitted); 
    var order = new Order
    {
      UserId = userId,
      CreatedAt = DateTime.UtcNow,
      OrderStatus = StatusOrder.Pending
    };
    try
    {
      var product = await productRepository.GetByIdAsync(createOrderDto.ProductId);
      if (product != null)
      {
        var orderItem = new OrderItem();
        product.DecreaseStock(createOrderDto.Quantity);
      
        orderItem.ProductId = product.Id;
        orderItem.Quantity = createOrderDto.Quantity;
        orderItem.Price = product.Price;
      
        order.Items.Add(orderItem);
        
        await orderRepository.AddAsync(order);
        await unitOfWork.CommitAsync();

        logger.LogInformation("Заказ {OrderId} успешно создан", order.Id);
        
        await eventBus.PublishAsync("order.created", new
        {
          orderId = order.Id,
          userId = order.UserId,
          Total = order.Items
            .Sum(i => i.Quantity * i.Price)
        });
      }
      else
      {
        logger.LogWarning("Товар {ProductId} не найден", createOrderDto.ProductId);
        throw new NotFoundException("Product not found", createOrderDto.ProductId);
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "Ошибка при создании заказа для пользователя {UserId}", userId); // ✅
      await unitOfWork.RollbackAsync();
      throw;
    }

    
    return ConvertToOrderDto(order);
  }

  public async Task<OrderDto> GetOrder(Guid idOrder)
  {
    var order = await orderRepository.GetAsync(idOrder);
    return ConvertToOrderDto(order);
  }

  public async Task<List<OrderDto>> GetByUserId(Guid userId)
  {
    var order = await orderRepository.GetByUserIdAsync(userId);
    var listOrder = order.Select(ConvertToOrderDto).ToList();
    return listOrder;
  }

  public async Task CancelOrder(Guid idOrder)
  {
    var order =  await orderRepository.GetAsync(idOrder);
    if (order != null)
    {
      if (order.OrderStatus == StatusOrder.Pending)
      {
        order.OrderStatus = StatusOrder.Cancelled;
        await orderRepository.UpdateAsync(order);
        await notification.NotifyOrderStatusChanged(idOrder, "Cancelled");
        logger.LogInformation("Заказ отменен {IdOrder}", idOrder);
      }
      else
        throw new DomainException("Order cannot be cancelled.");
    }
    else
      throw new NotFoundException("Order not found.",idOrder);
  }

  private OrderDto ConvertToOrderDto(Order order)
  {
    var itemDto = order.Items.Select(ConvertToOrderItemDto).ToList();
    var orderDto = new OrderDto(order.Id, order.UserId,itemDto,order.OrderStatus, order.CreatedAt);
    return orderDto;
  }

  private OrderItemDto ConvertToOrderItemDto(OrderItem orderItem)
  {
    var item = new OrderItemDto(orderItem.ProductId,orderItem.Product.Name, orderItem.Quantity, orderItem.Price);
    return item;
  }
}