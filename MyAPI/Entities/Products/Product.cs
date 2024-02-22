namespace MyAPI.Entities.Products;

public partial class Product : Entity
{
  public string Name { get; set; }
  public string Description { get; set; }
  public Category Category { get; set; }  
  public bool HasStock { get; set; }
  public decimal Price { get; set; }
  public ICollection<Order> Orders { get; set; }

  public void Update(
    string name,
    string description,
    Category category,
    bool hasStock,
    decimal price,
    string updatedBy
  ){
    Name = name;
    Description = description;
    Category = category;
    HasStock = hasStock;
    Price = price;
    UpdatedBy = updatedBy;
    UpdatedAt = DateTime.Now;
  }

}
