namespace MyAPI.Entities.Products;

public interface IProductRepository
{ 
  public Task<RepositoryResult> AddEntity(ProductDTO entity, string userId);
  public Task<RepositoryResult> RemoveEntity(Guid id);
  public Task<RepositoryResult> UpdateEntity(Guid id, ProductDTO entity, string userId);
  public Task<RepositoryResult> ActivateEntity(Guid id, bool status, string userId);
  public Task<ProductRDTO?> GetById(Guid id);
  public Task<GetAllRDTO<ProductRDTO>> GetAll(
    int? limit,
    int? offset,
    Guid? categoryId,
    string? searchTerm,
    bool? showCase,
    string orderBy,
    bool desc
  );
}
