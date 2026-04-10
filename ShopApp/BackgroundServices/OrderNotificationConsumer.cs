using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ShopApp.BackgroundServices;

public class OrderNotificationConsumer(IConfiguration configuration) : BaseConsumer(configuration)
{
    private string _orderName = "order.created";

    protected override async Task ConsumeAsync(CancellationToken stoppingToken)
    {
        await _channel.QueueDeclareAsync(queue: _orderName, durable: true, exclusive: false, 
            autoDelete: false, arguments: null, cancellationToken: stoppingToken);
        
        var custom = new AsyncEventingBasicConsumer(_channel);
        custom.ReceivedAsync += (sender, args) =>
        {
            var context = Encoding.UTF8.GetString(args.Body.ToArray());
            Console.WriteLine($"📦 Новый заказ: {context}");
            return Task.CompletedTask;
        }; 
        await _channel.BasicConsumeAsync(_orderName, true, custom, cancellationToken: stoppingToken);
    }
}