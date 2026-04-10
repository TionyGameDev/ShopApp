using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.DTOs.Auth;
using ShopApp.Application.Services.AuthServices;

namespace ShopApp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("login")]
    [EndpointSummary("Логин")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto login)
    {
        var loginResponse = await service.Login(login);
        return Ok(loginResponse);
    }
    [HttpPost("register")]
    [EndpointSummary("Регистрация")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto register)
    {
        var registerResponse = await service.RegisterUser(register);
        return Ok(registerResponse);
    }
}