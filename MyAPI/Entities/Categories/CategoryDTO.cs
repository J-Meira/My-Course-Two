using Flunt.Validations;

namespace MyAPI.Entities.Categories;

public partial class CategoryDTO : EntityDTO
{
  public string Name { get; set; }

  public CategoryDTO(string name)
  {
    Name = name;
    Validate();
  }

  private void Validate()
  {
    Contract<CategoryDTO> contract = new Contract<CategoryDTO>()
      .IsNotNullOrEmpty(Name, "Name")
      .IsGreaterOrEqualsThan(Name, 3, "Name");
    AddNotifications(contract);
  }
}
