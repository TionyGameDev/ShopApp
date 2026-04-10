using ShopApp.Application.DTOs.Auth;
using ShopApp.Application.Interfaces;
using ShopApp.Domain.Entites;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Services.AuthServices;

public class AuthService : IAuthService
{
    private IAuthRepository _repository;
    private IJwtService _jwtService;

    public AuthService(IAuthRepository  repository,IJwtService jwtService)
    {
        _jwtService = jwtService;
        _repository = repository;
    }

    public async Task<AuthResponseDto> RegisterUser(RegisterDto model)
    {
        var user = await _repository.GetUserByEmail(model.Email);
    
        if (user == null)
        {
            var userAdd  = new User
            {
                Email = model.Email,
                Name = model.Name,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password)
            };
      
            await _repository.AddAsync(userAdd);
            var authResponce = new AuthResponseDto(true, "Registration Successful", "", "");
      
            return authResponce;
        }

        throw new DomainException("User already exists");

    }

    public async Task<AuthResponseDto> Login(LoginDto model)
    {
        var user = await _repository.GetUserByEmail(model.Email);
        if (user != null)
        {
            if (BCrypt.Net.BCrypt.Verify(model.Password,user.PasswordHash))
            {
                var token = _jwtService.GenerationToken(user.Id,user.Role);
                var responce = new AuthResponseDto(true, "Login Successful", "", token);
                return responce;
            }

            throw new DomainException("Invalid login");
      
        }
    
        throw new DomainException("User not found");
    
    }
}