using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using ShopApp.Application.Interfaces;

namespace ShopApp.Application.Services.Kafka;

public class KafkaEventBus :  IEventBus
{
    private readonly IProducer<string,string> _producer;
    
    public KafkaEventBus(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }
    
    public async Task PublishAsync<T>(string topic, T message)
    {
        var json = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = json
        });
    }
}