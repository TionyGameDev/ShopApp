namespace ShopApp.Domain.Entites;

public class Order
{
  public Guid Id { get; set; }
  public Guid UserId { get; set; }
  public User User { get; set; }
  public List<OrderItem> Items { get; set; }
  public StatusOrder OrderStatus { get; set; }
  public DateTime CreatedAt { get; set; }
}

public class OrderItem    
{
  public Guid Id { get; set; }
  public Guid OrderId { get; set; }
  public Guid ProductId { get; set; }
  public Product Product { get; set; }
  public int Quantity { get; set; }
  public decimal Price { get; set; }
  
}