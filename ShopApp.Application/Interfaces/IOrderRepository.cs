using ShopApp.Domain.Entites;

namespace ShopApp.Application.Interfaces;

public interface IOrderRepository
{
  Task AddAsync(Order order);
  Task RemoveAsync(Order order);
  Task UpdateAsync(Order order);
  Task<List<Order>> GetAllAsync();
  Task<Order> GetAsync(Guid idOrder);
  Task<List<Order>> GetByUserIdAsync(Guid userId);
}