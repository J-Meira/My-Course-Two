namespace MyAPI.Infra.DataBase;

public class AppDbContext : IdentityDbContext<IdentityUser>
{
  public DbSet<Product> Products { get; set; }
  public DbSet<Category> Categories { get; set; }
  public DbSet<Client> Clients { get; set; }
  public DbSet<Employee> Employees { get; set; }
  public DbSet<Order> Orders { get; set; }

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    modelBuilder.Ignore<Notification>();

    modelBuilder.HasDefaultSchema("MySchema");

    modelBuilder
      .Entity<Product>()
      .Property(p => p.Description)
      .HasMaxLength(255);

    modelBuilder
      .Entity<Order>()
      .HasMany(o => o.Products)
      .WithMany(p => p.Orders)
      .UsingEntity(x => x.ToTable("OrderProducts"));
  }

  protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
  {
    configurationBuilder
      .Properties<string>()
      .HaveMaxLength(100);
    configurationBuilder
      .Properties<decimal>()
      .HaveColumnType("decimal(10,2)");
  }
}
