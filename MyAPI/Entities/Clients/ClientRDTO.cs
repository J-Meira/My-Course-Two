namespace MyAPI.Entities.Clients;

public partial class ClientRDTO
{
  public Guid Id { get; set; }
  public string Name {get; set;}
  public string Email {get; set;}
  public string NationalRegistrationNumber { get; set;}
  public bool Active {get; set;}
}
