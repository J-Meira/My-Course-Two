namespace MyAPI.Entities.Categories;

public interface ICategoryRepository
{
  public bool AddEntity(CategoryDTO entity);
  public bool RemoveEntity(Guid id);
  public bool UpdateEntity(Guid id, CategoryDTO entity);
  public bool ActivateEntity(Guid id, bool status);
  public CategoryRDTO? GetById(Guid id);
  public IEnumerable<CategoryRDTO> GetAll();
}
