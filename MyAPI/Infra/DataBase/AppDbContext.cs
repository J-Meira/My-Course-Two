using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;
using MyAPI.Entities.Categories;
using MyAPI.Entities.Products;

namespace MyAPI.Infra.DataBase;

public class AppDbContext : DbContext
{
  public DbSet<Product> Products { get; set; }
  public DbSet<Category> Categories { get; set; }

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Ignore<Notification>();

    modelBuilder
      .HasDefaultSchema("MySchema");

    modelBuilder
      .Entity<Product>()
      .Property(p => p.Description)
      .HasMaxLength(255);
  }

  protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
  {
    configurationBuilder
      .Properties<string>()
      .HaveMaxLength(100);
  }
}
