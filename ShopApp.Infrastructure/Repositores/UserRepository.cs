using Microsoft.EntityFrameworkCore;
using ShopApp.Application.DTOs.Auth;
using ShopApp.Application.Interfaces;
using ShopApp.Domain.Entites;
using ShopApp.Infrastructure.Data;

namespace ShopApp.Infrastructure.Repositores;

public class UserRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}