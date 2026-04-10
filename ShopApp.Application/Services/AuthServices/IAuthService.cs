using ShopApp.Application.DTOs.Auth;

namespace ShopApp.Application.Services.AuthServices;

public interface IAuthService
{
  Task<AuthResponseDto> RegisterUser(RegisterDto model);
  Task<AuthResponseDto> Login(LoginDto model);
}