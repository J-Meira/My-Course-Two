namespace MyAPI.Entities.Orders;

public partial class OrderDTO : EntityDTO
{
  public List<Guid> Products { get; set; }

  public OrderDTO(List<Guid> products)
  {
    Products = products;
    Validate();
  }

  private void Validate()
  {
    Contract<OrderDTO> contract = new Contract<OrderDTO>()
      .IsTrue(Products != null && Products.Any(), "Products");

    AddNotifications(contract);
  }
}
