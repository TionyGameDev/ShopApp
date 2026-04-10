using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopApp.Application.Services.AuthServices;

namespace ShopApp.Infrastructure.Services;

public class JwtService : IJwtService
{
  private readonly string _secret;
  public JwtService(IConfiguration configuration)
  {
    _secret = configuration["Jwt:Secret"] ?? string.Empty;
  }

  public string GenerationToken(Guid Id,string role = "")
  {
    var bytes = System.Text.Encoding.UTF8.GetBytes(_secret);
    var key = new SymmetricSecurityKey(bytes); 
    var credentials  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var claims = new List<Claim>();
    
    claims.Add(new Claim(ClaimTypes.Role, role));
    claims.Add(new Claim("id",Id.ToString()));
        
    JwtSecurityToken token = new JwtSecurityToken(
      expires: DateTime.UtcNow.AddHours(10),
      signingCredentials: credentials,
      claims: claims);
        
    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}