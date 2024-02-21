namespace MyAPI.Entities.Employees;

public class EmployeeDTO : EntityDTO
{
  public string Name { get; set; }
  public string Registration { get; set; }
  public string Email { get; set; }
  public string Password { get; set; }

  public EmployeeDTO(string name, string registration, string email, string password)
  {
    Name = name;
    Registration = registration;
    Email = email;
    Password = password;
    Validate();
  }

  private void Validate()
  {
    Contract<EmployeeDTO> contract = new Contract<EmployeeDTO>()
      .IsNotNullOrEmpty(Name, "Name")
      .IsGreaterOrEqualsThan(Name, 3, "Name")
      .IsNotNullOrEmpty(Email, "Email")
      .IsEmail(Email, "Email")
      .IsNotNullOrEmpty(Password, "Password")
      .IsGreaterOrEqualsThan(Password, 10, "Password")
      .IsNotNullOrEmpty(Registration, "Registration");

    AddNotifications(contract);
  }
}
