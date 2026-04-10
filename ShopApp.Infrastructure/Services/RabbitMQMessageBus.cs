using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using ShopApp.Application.Interfaces;

namespace ShopApp.Infrastructure.Services;

public class RabbitMQMessageBus : IMessageBus ,  IAsyncDisposable
{
    private IConnection _connection;
    private IChannel _channel;
    private CancellationToken _cancellationToken;
    
    public static async Task<RabbitMQMessageBus> CreateAsync(IConfiguration configuration)
    {
        var cts = new CancellationTokenSource();
        
        var bus = new RabbitMQMessageBus();
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"],
            Port = int.Parse(configuration["RabbitMQ:Port"]),
            UserName = configuration["RabbitMQ:Username"],
            Password = configuration["RabbitMQ:Password"]
        };

        bus._cancellationToken = cts.Token;
        bus._connection = await factory.CreateConnectionAsync(cts.Token);
        bus._channel = await bus._connection.CreateChannelAsync(cancellationToken: cts.Token);

        return bus;
    }
    public async ValueTask DisposeAsync()
    {
        await _channel.CloseAsync(cancellationToken: _cancellationToken);
        await _connection.CloseAsync(cancellationToken: _cancellationToken);
    }
    
    public async Task PublishAsync<T>(string queue, T message)
    {
      var declare = await _channel.QueueDeclareAsync(queue, true, false, 
          false, cancellationToken: _cancellationToken);
      var json = JsonSerializer.Serialize(message);
      var body = Encoding.UTF8.GetBytes(json);
      await _channel.BasicPublishAsync(
          exchange: "",
          routingKey: declare.QueueName, 
          body: body, 
          cancellationToken: _cancellationToken);
    }
    
}