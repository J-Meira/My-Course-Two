
using Microsoft.AspNetCore.Identity;

namespace MyAPI.Entities.Employees;

public partial class Employee : Entity
{
  public string Name { get; set; }
  public IdentityUser User { get; set; }
  public string UserId { get; set; }
  public string Registration { get; set; }

  public void Update(string name, string registration, string updatedBy)
  {
     Name = name;
     Registration = registration;
     UpdatedBy = updatedBy;
     UpdatedAt = DateTime.Now;
  }
}
