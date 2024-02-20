namespace MyAPI.Entities.Categories;

public partial class Category : Entity
{
  public string Name { get; set; }
  
  public void Update(
    string name,
    string updatedBy
  ){
     Name = name;
     UpdatedBy = updatedBy;
     UpdatedAt = DateTime.Now;
  }
}
