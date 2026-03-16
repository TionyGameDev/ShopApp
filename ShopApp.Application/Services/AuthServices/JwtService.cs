using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ShopApp.Application.Services.AuthServices;

public interface IJwtService
{
  string GenerationToken(Guid Id,string role = "");
}

public class JwtService : IJwtService
{
  private readonly string _secret;
  public JwtService(IConfiguration configuration)
  {
    _secret = configuration["Jwt:Secret"];
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
      expires: DateTime.Now.AddHours(10),
      signingCredentials: credentials,
      claims: claims);
        
    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}