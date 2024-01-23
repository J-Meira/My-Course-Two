using Flunt.Validations;
using MyAPI.Entities.Categories;

namespace MyAPI.Entities.Employees;

public class EmployeeUpdateDTO : EntityDTO
{
  public string Name { get; set; }
  public string Registration { get; set; }
  public string Email { get; set; }

  public EmployeeUpdateDTO(string name, string registration, string email)
  {
    Name = name;
    Registration = registration;
    Email = email;
    Validate();
  }

  private void Validate()
  {
    Contract<CategoryDTO> contract = new Contract<CategoryDTO>()
      .IsNotNullOrEmpty(Name, "Name")
      .IsGreaterOrEqualsThan(Name, 3, "Name")
      .IsNotNullOrEmpty(Email, "Email")
      .IsEmail(Email, "Email")
      .IsNotNullOrEmpty(Registration, "Registration");

    AddNotifications(contract);
  }
}
