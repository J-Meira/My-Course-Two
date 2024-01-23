namespace MyAPI.Entities.Categories;

public interface ICategoryRepository
{
  public RepositoryTaskResult AddEntity(CategoryDTO entity, string userId);
  public RepositoryTaskResult RemoveEntity(Guid id);
  public RepositoryTaskResult UpdateEntity(Guid id, CategoryDTO entity, string userId);
  public RepositoryTaskResult ActivateEntity(Guid id, bool status, string userId);
  public CategoryRDTO? GetById(Guid id);
  public IEnumerable<CategoryRDTO> GetAll();
}
