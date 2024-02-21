namespace MyAPI.Entities.Employees;

public class ClientUpdateDTO : EntityDTO
{
  public string Name { get; set; }
  public string NationalRegistrationNumber { get; set; }
  public string Email { get; set; }

  public ClientUpdateDTO(string name, string nationalRegistrationNumber, string email)
  {
    Name = name;
    NationalRegistrationNumber = nationalRegistrationNumber;
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
      .IsNotNullOrEmpty(NationalRegistrationNumber, "Registration");

    AddNotifications(contract);
  }
}
