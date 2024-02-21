namespace MyAPI.Entities.Products;

public partial class ProductDTO : EntityDTO
{
  public string Name { get; set; }
  public string Description { get; set; }
  public Guid CategoryId { get; set; }  
  public bool HasStock { get; set; }
  public decimal Price { get; set; }

  public ProductDTO(
    string name,
    string description,
    Guid categoryId,
    bool hasStock,
    decimal price
  )
  {
    Name = name;
    Description = description;
    CategoryId = categoryId;
    HasStock = hasStock;
    Price = price;
    Validate();
  }

  private void Validate()
  {
    Contract<CategoryDTO> contract = new Contract<CategoryDTO>()
      .IsNotNullOrEmpty(Name, "Name")
      .IsGreaterOrEqualsThan(Name, 3, "Name")
      .IsNotNullOrEmpty(Description, "Description")
      .IsGreaterOrEqualsThan(Description, 10, "Description")
      .IsNotNull(CategoryId, "CategoryId")
      .IsNotNull(HasStock, "HasStock")
      .IsNotNull(Price, "Price")
      .IsGreaterOrEqualsThan(Price, 1, "Price");

    AddNotifications(contract);
  }
}
