using ShopApp.Application.DTOs.Auth;
using ShopApp.Domain.Entites;

namespace ShopApp.Application.Interfaces;

public interface IAuthRepository
{
  Task<User> Login(LoginDto model);
  Task<User> AddAsync(User user);
  
  Task<User> GetUserByEmail(string email);
}