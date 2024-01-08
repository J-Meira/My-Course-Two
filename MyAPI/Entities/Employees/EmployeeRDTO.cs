namespace MyAPI.Entities.Employees;

public partial class EmployeeRDTO
{
  public Guid Id { get; set; }
  public string Name {get; set;}
  public string Email {get; set;}
  public string Registration {get; set;}
  public bool Active {get; set;}
}
