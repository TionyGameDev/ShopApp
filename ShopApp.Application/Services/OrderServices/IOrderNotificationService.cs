using Microsoft.AspNetCore.SignalR;

namespace ShopApp.Application.Services.OrderServices;

public interface IOrderNotificationService
{
    Task NotifyOrderStatusChanged(Guid orderId, string status);
}

