using MyAPI.Entities.Categories;
namespace MyAPI.Entities.Products;

public partial class Product : Entity
{
  public string Name { get; set; }
  public string? Description { get; set; }
  public Category Category { get; set; }  
  public bool HasStock { get; set; }
}
