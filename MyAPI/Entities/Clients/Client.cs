namespace MyAPI.Entities.Clients;

public partial class Client : Entity
{
  public string Name { get; set; }
  public IdentityUser User { get; set; }
  public string UserId { get; set; }
  public string NationalRegistrationNumber { get; set; }

  public void Update(
    string name,
    string nationalRegistrationNumber,
    string updatedBy
  ){
     Name = name;
    NationalRegistrationNumber = nationalRegistrationNumber;
     UpdatedBy = updatedBy;
     UpdatedAt = DateTime.Now;
  }
}
