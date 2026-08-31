using Microsoft.EntityFrameworkCore;

namespace MyShop.Models;

public class ItemDbContext : DbContext
{
	public ItemDbContext(DbContextOptions<ItemDbContext> options) : base(options)
	{
        Database.EnsureCreated(); // For early prototyping only. Remove when switching to EF Core Migrations.
	}

	public DbSet<Item> Items { get; set; }
}

