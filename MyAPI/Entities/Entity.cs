namespace MyAPI.Entities;

public abstract class Entity
{
  public Guid Id { get; set; }
  public bool Active { get; set; } = true;
  public string CreatedBy { get; set; }
  public DateTime CreatedAt { get; set; }
  public string UpdatedBy { get; set; }
  public DateTime UpdatedAt { get; set; }

  public void Activate(bool active, string updatedBy)
  {
    UpdatedBy = updatedBy;
    UpdatedAt = DateTime.Now;
    Active = active;
  }
}
