namespace ShopApp.Application.Interfaces;

public interface IEventBus
{
    Task PublishAsync<T>(string topic, T message);
}