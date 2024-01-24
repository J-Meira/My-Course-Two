namespace MyAPI.Entities.Categories;

public interface ICategoryRepository
{
  public Task<RepositoryResult> AddEntity(CategoryDTO entity, string userId);
  public Task<RepositoryResult> RemoveEntity(Guid id);
  public Task<RepositoryResult> UpdateEntity(Guid id, CategoryDTO entity, string userId);
  public Task<RepositoryResult> ActivateEntity(Guid id, bool status, string userId);
  public Task<CategoryRDTO?> GetById(Guid id);
  public Task<IEnumerable<CategoryRDTO>> GetAll();
}
