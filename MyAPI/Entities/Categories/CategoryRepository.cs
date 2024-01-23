using MyAPI.Infra.DataBase;

namespace MyAPI.Entities.Categories;

public class CategoryRepository : ICategoryRepository
{
  private readonly AppDbContext _context;

  public CategoryRepository(AppDbContext context)
  {
    _context = context;
  }
  public RepositoryTaskResult AddEntity(CategoryDTO entity, string userId)
  {
    Category category = new()
    {
      Name = entity.Name,
      CreatedBy = userId,
      CreatedAt = DateTime.Now,
      UpdatedBy = userId,
      UpdatedAt = DateTime.Now,
    };
    _context.Categories.Add(category);

    int resultSave = _context.SaveChanges();

    return new RepositoryTaskResult
    {
      Success = resultSave > 0,
      Errors = null
    };
  }
  public RepositoryTaskResult RemoveEntity(Guid id)
  {
    Category? dbEntity = _context.Categories.FirstOrDefault(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryTaskResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category nout found")
      };
    }
    _context.Categories.Remove(dbEntity);

    int resultSave = _context.SaveChanges();

    return new RepositoryTaskResult
    {
      Success = resultSave > 0,
      Errors = null
    };
  }
  public RepositoryTaskResult UpdateEntity(Guid id, CategoryDTO entity, string userId)
  {
    Category? dbEntity = _context.Categories.FirstOrDefault(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryTaskResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category nout found")
      };
    }
    dbEntity.Update(entity.Name, userId);

    int resultSave = _context.SaveChanges();

    return new RepositoryTaskResult
    {
      Success = resultSave > 0,
      Errors = null
    };
  }
  public CategoryRDTO? GetById(Guid id)
  {
    Category? entity = _context.Categories.Where(x => x.Id == id).FirstOrDefault();
    return entity != null ? new CategoryRDTO()
    {
      Id = entity.Id,
      Name = entity.Name,
      Active = entity.Active,

    } : null;
  }
  public IEnumerable<CategoryRDTO> GetAll()
  {
    IEnumerable<Category> list = _context.Categories.ToList();
    return list.Select(c => new CategoryRDTO { Id = c.Id, Name = c.Name, Active = c.Active });
  }
  public RepositoryTaskResult ActivateEntity(Guid id, bool status, string userId)
  {
    Category? dbEntity = _context.Categories.FirstOrDefault(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryTaskResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category nout found")
      };
    }
    dbEntity.Activate(status, userId);
    int resultSave = _context.SaveChanges();

    return new RepositoryTaskResult
    {
      Success = resultSave > 0,
      Errors = null
    };
  }
}
