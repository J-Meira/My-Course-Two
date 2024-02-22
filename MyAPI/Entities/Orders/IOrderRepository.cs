namespace MyAPI.Entities.Orders;

public interface IOrderRepository
{
  public Task<RepositoryResult> AddEntity(OrderDTO entity, string userId);
  public Task<RepositoryResult> Finish(Guid id, string userId);
  public Task<OrderRDTO?> GetById(Guid id);
  public Task<GetAllRDTO<OrderReportRDTO>> GetAll(
    int? limit,
    int? offset,
    Guid? clientId,
    string orderBy,
    bool desc
  );
}
