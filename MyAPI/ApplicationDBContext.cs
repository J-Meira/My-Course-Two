using Microsoft.EntityFrameworkCore;

public class ApplicationDBContext : DbContext
{

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options): base (options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("MySchema");

        modelBuilder.Entity<Product>()
        .Property(p=> p.Description).HasMaxLength(200);
        modelBuilder.Entity<Product>()
        .Property(p=> p.Code).HasMaxLength(20);
        modelBuilder.Entity<Product>()
        .Property(p=> p.Name).HasMaxLength(100);
        
        modelBuilder.Entity<Category>()
        .Property(p=> p.Name).HasMaxLength(100);
        
        modelBuilder.Entity<Tag>()
        .Property(p=> p.Name).HasMaxLength(100);
    }
}
