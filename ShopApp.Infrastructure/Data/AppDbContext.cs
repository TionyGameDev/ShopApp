  using Microsoft.EntityFrameworkCore;
  using ShopApp.Domain.Entites;

  namespace ShopApp.Infrastructure.Data;

  public class AppDbContext : DbContext
  {
    public DbSet<User> Users  { get; set; }
    public DbSet<Order> Orders   { get; set; }
    public DbSet<OrderItem> OrderItems   { get; set; }
    public DbSet<Product> Products   { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
          
    }

    override protected void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      
      CreateUser(modelBuilder);
      CreateOrder(modelBuilder);
      CreateOrderItems(modelBuilder);
      CreateProduct(modelBuilder);
    }

    private void CreateProduct(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Product>()
        .HasIndex(p => p.Name);
    }

    private void CreateOrderItems(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<OrderItem>()
        .HasKey(x => new { x.OrderId, x.ProductId });

      modelBuilder.Entity<Order>()
        .HasMany(d => d.Items)
        .WithOne()
        .HasForeignKey(x => x.OrderId);
      
      modelBuilder.Entity<OrderItem>()
        .HasOne(d => d.Product)
        .WithMany()
        .HasForeignKey(x => x.ProductId);

    }

    private void CreateOrder(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Order>()
        .HasOne(x => x.User)
        .WithMany()
        .HasForeignKey(x => x.UserId);
    }

    private void CreateUser(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();
    }

  }