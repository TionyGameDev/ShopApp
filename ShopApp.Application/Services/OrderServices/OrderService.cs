using ShopApp.Application.DTOs.Orders;
using ShopApp.Application.Interfaces;
using ShopApp.Domain.Entites;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Services.OrderServices;

public class OrderService : IOrderService
{
  private readonly IOrderRepository _orderRepository;
  private IProductRepository  _productRepository;
  private readonly IMessageBus _messageBus;

  public OrderService(IOrderRepository orderRepository,IProductRepository  productRepository,IMessageBus  messageBus)
  {
    _productRepository = productRepository;
    _messageBus = messageBus;
    _orderRepository = orderRepository;
  }

  public async Task<OrderDto> CreateOrder(CreateOrderDto createOrderDto, Guid userId)
  {
    var order = new Order();
    order.UserId = userId;
    order.CreatedAt = DateTime.UtcNow;
    order.OrderStatus = StatusOrder.Pending;
    var product = await _productRepository.GetByIdAsync(createOrderDto.ProductId);
    if (product != null)
    {
      var orderItem = new OrderItem();
      product.DecreaseStock(createOrderDto.Quantity);
      
      orderItem.ProductId = product.Id;
      orderItem.Quantity = createOrderDto.Quantity;
      orderItem.Price = product.Price;
      
      order.Items.Add(orderItem);
      await _orderRepository.AddAsync(order);
    }
    else
      throw new NotFoundException("Product not found", createOrderDto.ProductId);

    await _messageBus.PublishAsync("order.created", new
    {
      orderId = order.Id,
      userId = order.UserId,
      Total = order.Items.Sum(i => i.Quantity * i.Price)
    });
    return ConvertToOrderDto(order);
  }

  public async Task<OrderDto> GetOrder(Guid idOrder)
  {
    var order = await _orderRepository.GetAsync(idOrder);
    return ConvertToOrderDto(order);
  }

  public async Task<List<OrderDto>> GetByUserId(Guid userId)
  {
    var order = await _orderRepository.GetByUserIdAsync(userId);
    var listOrder = order.Select(ConvertToOrderDto).ToList();
    return listOrder;
  }

  public async Task CancelOrder(Guid idOrder)
  {
    var order =  await _orderRepository.GetAsync(idOrder);
    if (order != null)
    {
      if (order.OrderStatus == StatusOrder.Pending)
      {
        order.OrderStatus = StatusOrder.Cancelled;
        await _orderRepository.UpdateAsync(order);
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