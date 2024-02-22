namespace MyAPI.Entities.Orders;

public partial class OrderRDTO : OrderReportRDTO
{
  public List<ProductRDTO> Products { get; set; }

  public OrderRDTO(Guid id, ClientRDTO client, List<ProductRDTO> products)
  {
    Id = id;
    Client = client;
    Products = products;
    Total = 0;
    foreach (var item in Products)
    {
      Total += item.Price;
    }
  }

}
