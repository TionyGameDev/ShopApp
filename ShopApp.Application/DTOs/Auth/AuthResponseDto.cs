namespace ShopApp.Application.DTOs.Auth;

public record AuthResponseDto(bool Success,string Message,string Error,string JwtToken);