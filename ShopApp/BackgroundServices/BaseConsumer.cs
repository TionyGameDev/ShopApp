using RabbitMQ.Client;

namespace ShopApp.BackgroundServices;

public abstract class BaseConsumer(IConfiguration configuration) : BackgroundService
{
    protected readonly IConfiguration _configuration = configuration;
    protected IChannel _channel;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"],
            Port = int.Parse(_configuration["RabbitMQ:Port"]),
            UserName = _configuration["RabbitMQ:Username"],
            Password = _configuration["RabbitMQ:Password"]
        };
        
        var connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);
        
        await ConsumeAsync(stoppingToken);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
    
    protected abstract Task ConsumeAsync(CancellationToken stoppingToken);
}