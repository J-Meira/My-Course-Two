using Flunt.Validations;

namespace MyAPI.Entities;

public class SignInDTO :EntityDTO
{
  public string Email { get; set; }
  public string Password { get; set; }

  public SignInDTO(string email, string password)
  {
    Email = email;
    Password = password;
    Validate();
  }

  private void Validate()
  {
    Contract<SignInDTO> contract = new Contract<SignInDTO>()
      .IsNotNullOrEmpty(Email, "Email")
      .IsEmail(Email, "Email")
      .IsNotNullOrEmpty(Password, "Password");
    AddNotifications(contract);
  }

}
