namespace MyAPI.Entities.Products;

public partial class ProductRDTO
{
  public Guid Id { get; set; }
  public string Name { get; set; }
  public string Description { get; set; }
  public CategoryRDTO Category { get; set; }
  public bool HasStock { get; set; }
  public decimal Price { get; set; }
  public bool Active { get; set; }
}
