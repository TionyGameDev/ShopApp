using Microsoft.EntityFrameworkCore;
using ShopApp.Application.Interfaces;
using ShopApp.Domain.Entites;
using ShopApp.Infrastructure.Data;

namespace ShopApp.Infrastructure.Repositores;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context) => _context = context;

    public Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        return _context.SaveChangesAsync();
    }

    public Task RemoveAsync(Order order)
    {
        _context.Orders.Remove(order);
        return _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Order>> GetAllAsync() => await _context.Orders
        .Include(x => x.Items)
            .ThenInclude(p => p.Product)
        .ToListAsync();

    public async Task<Order?> GetAsync(Guid idOrder)
    {
      return await _context.Orders
          .Include(x => x.Items)
            .ThenInclude(p => p.Product)
          .FirstOrDefaultAsync(x => x.Id == idOrder);
    }

    public async Task<List<Order>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Include(x => x.Items)
                .ThenInclude(p => p.Product)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }
}