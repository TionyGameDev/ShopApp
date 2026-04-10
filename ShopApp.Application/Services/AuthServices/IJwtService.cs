namespace ShopApp.Application.Services.AuthServices;

public interface IJwtService
{
    string GenerationToken(Guid Id,string role = "");
}