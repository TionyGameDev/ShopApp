using ShopApp.Domain.Entites;

namespace ShopApp.Application.Interfaces;

public class ProductRepository : IProductRepository
{

  public Task<Product> AddAsync(Product product) => throw new NotImplementedException();

  public Task RemoveAsync(Guid idProduct) => throw new NotImplementedException();

  public Task UpdateAsync(Product product) => throw new NotImplementedException();

  public Task<List<Product>> GetAllAsync() => throw new NotImplementedException();

  public Task<Product> GetByIdAsync(Guid idProduct) => throw new NotImplementedException();
}