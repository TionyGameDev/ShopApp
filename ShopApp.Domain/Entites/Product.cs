namespace ShopApp.Domain.Entites;

public class Product
{
  public Guid Id { get; set; }
  public string Name { get; set; } = null!;
  public string Description { get; set; } = null!;
  public decimal Price { get; set; }
  public int Stock { get; set; }
  
  public bool IsAvailable() =>  Stock > 0;

  public void DecreaseStock(int amount)
  {
    if (Stock >= amount)
      Stock -= amount;
    else 
      throw new InvalidOperationException("Not enough stock");
  }
}
