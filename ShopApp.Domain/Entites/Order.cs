namespace ShopApp.Domain.Entites;

public class Order
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public User User { get; set; }
  public List<OrderItem> Items { get; set; } = new List<OrderItem>();
  public StatusOrder OrderStatus { get; set; }
  public DateTime CreatedAt { get; set; } =  DateTime.UtcNow;
}