namespace MyAPI.Entities.Orders;

public partial class Order
{
  public Guid Id { get; set; }
  public Client Client { get; set; }
  public List<Product> Products { get; set; }
  public decimal Total { get; set; }
  public string CreatedBy { get; set; }
  public DateTime CreatedAt { get; set; }
  public string UpdatedBy { get; set; }
  public DateTime UpdatedAt { get; set; }
  public string? FinishedBy { get; set; }
  public DateTime? FinishedAt { get; set; }

  public void UpdateTotal()
  {
    Total = 0;
    foreach (var item in Products)
    {
      Total += item.Price;
    }
  }

  public void Finish(string userId)
  {
    FinishedBy = userId;
    UpdatedBy = userId;
    FinishedAt = DateTime.Now;
    UpdatedAt = DateTime.Now;
  }


}
