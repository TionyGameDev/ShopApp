namespace ShopApp.Domain.Entites;

public class User
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Email { get; set; } = string.Empty;
  public string PasswordHash { get; set; }
  public string Role { get; set; } = "Default";
  public DateTime CreatedAt { get; set; }

  public bool IsAdmin() => Role == "Admin";

}