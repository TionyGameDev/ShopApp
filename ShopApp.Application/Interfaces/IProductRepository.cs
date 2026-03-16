using ShopApp.Domain.Entites;

namespace ShopApp.Application.Interfaces;

public interface IProductRepository
{
  Task<Product> AddAsync(Product product);
  Task RemoveAsync(Guid idProduct);
  Task UpdateAsync(Product product);
  
  Task<List<Product>> GetAllAsync();
  Task<Product?> GetByIdAsync(Guid idProduct);
}