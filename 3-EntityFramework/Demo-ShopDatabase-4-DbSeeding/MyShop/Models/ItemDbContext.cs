using Microsoft.EntityFrameworkCore;

namespace MyShop.Models;

public class ItemDbContext : DbContext
{
	public ItemDbContext(DbContextOptions<ItemDbContext> options) : base(options)
	{
        // Database.EnsureCreated();  // For early prototyping only. Remove when switching to EF Core Migrations.
	}

	public DbSet<Item> Items { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }
}

