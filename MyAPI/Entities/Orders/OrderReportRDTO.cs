namespace MyAPI.Entities.Orders;

public partial class OrderReportRDTO
{
  public Guid Id { get; set; }
  public ClientRDTO Client { get; set; }
  public DateTime CreatedAt { get; set; }
  public decimal Total { get; set; }
}
