namespace MyAPI.Entities.Clients;

public class ClientDTO : EntityDTO
{
  public string Name { get; set; }
  public string NationalRegistrationNumber { get; set; }
  public string Email { get; set; }
  public string Password { get; set; }

  public ClientDTO(string name, string nationalRegistrationNumber, string email, string password)
  {
    Name = name;
    NationalRegistrationNumber = nationalRegistrationNumber;
    Email = email;
    Password = password;
    Validate();
  }

  private void Validate()
  {
    Contract<ClientDTO> contract = new Contract<ClientDTO>()
      .IsNotNullOrEmpty(Name, "Name")
      .IsGreaterOrEqualsThan(Name, 3, "Name")
      .IsNotNullOrEmpty(Email, "Email")
      .IsEmail(Email, "Email")
      .IsNotNullOrEmpty(Password, "Password")
      .IsGreaterOrEqualsThan(Password, 10, "Password")
      .IsNotNullOrEmpty(NationalRegistrationNumber, "Registration");

    AddNotifications(contract);
  }
}
