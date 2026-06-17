using Microsoft.AspNetCore.SignalR;
using ShopApp.Application.Services.OrderServices;
using ShopApp.Hubs;

namespace ShopApp.Services;

public class OrderNotificationService : IOrderNotificationService
{
    private readonly IHubContext<OrderHub> _hubContext;

    public OrderNotificationService(IHubContext<OrderHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyOrderStatusChanged(Guid orderId, string status)
    {
        Console.WriteLine($"🔔 Отправляем уведомление для заказа {orderId}: {status}");
        
        await _hubContext.Clients
            .Group(orderId.ToString())
            .SendAsync("OrderStatusChanged", new
            {
                OrderId = orderId,
                Status = status
            });
    }
}