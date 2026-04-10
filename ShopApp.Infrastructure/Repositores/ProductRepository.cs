using Microsoft.EntityFrameworkCore;
using ShopApp.Application.Interfaces;
using ShopApp.Domain.Entites;
using ShopApp.Infrastructure.Data;

namespace ShopApp.Infrastructure.Repositores;

public class ProductRepository : IProductRepository
{
  private readonly AppDbContext _context;
  public ProductRepository(AppDbContext context) => _context = context;

  public async Task<Product> AddAsync(Product product)
  {
    _context.Products.Add(product);
    await _context.SaveChangesAsync();
    return product;
  }

  public async Task RemoveAsync(Guid idProduct)
  {
    _context.Products.Remove(await _context.Products.FindAsync(idProduct) 
                             ?? throw new Exception("Product not found"));
    await _context.SaveChangesAsync();
  }

  public async Task UpdateAsync(Product product)
  {
    _context.Products.Update(product);
    await _context.SaveChangesAsync();
  }

  public async Task<List<Product>> GetAllAsync()
  {
    return await _context.Products.ToListAsync();
  }

  public async Task<Product?> GetByIdAsync(Guid idProduct) => await _context.Products.FindAsync(idProduct);
}