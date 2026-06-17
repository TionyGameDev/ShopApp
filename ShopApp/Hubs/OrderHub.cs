using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace ShopApp.Hubs;

public class OrderHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        Log.Information("✅ Клиент подключился: {ConnectionId}",Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public async Task JoinOrderGroup(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
        Log.Information("📦 Клиент {ConnectionId} подписался на заказ {orderId}", 
            Context.ConnectionId, orderId);
    }
}