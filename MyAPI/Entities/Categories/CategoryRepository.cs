namespace MyAPI.Entities.Categories;

public class CategoryRepository : ICategoryRepository
{
  private readonly AppDbContext _context;

  public CategoryRepository(AppDbContext context)
  {
    _context = context;
  }
  public async Task<RepositoryResult> AddEntity(CategoryDTO entity, string userId)
  {
    Category category = new()
    {
      Name = entity.Name,
      CreatedBy = userId,
      CreatedAt = DateTime.Now,
      UpdatedBy = userId,
      UpdatedAt = DateTime.Now,
    };
    await _context.Categories.AddAsync(category);

    int resultSave = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = resultSave > 0,
      Errors = null
    };
  }
  public async Task<RepositoryResult> RemoveEntity(Guid id)
  {
    Category? dbEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category nout found")
      };
    }
    _context.Categories.Remove(dbEntity);

    int resultSave = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = resultSave > 0,
      Errors = null
    };
  }
  public async Task<RepositoryResult> UpdateEntity(Guid id, CategoryDTO entity, string userId)
  {
    Category? dbEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category nout found")
      };
    }
    dbEntity.Update(entity.Name, userId);

    int resultSave = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = resultSave > 0,
      Errors = null
    };
  }
  public async Task<CategoryRDTO?> GetById(Guid id)
  {
    Category? entity = await _context.Categories.Where(x => x.Id == id).FirstOrDefaultAsync();
    return entity != null ? new CategoryRDTO()
    {
      Id = entity.Id,
      Name = entity.Name,
      Active = entity.Active,

    } : null;
  }
  public async Task<IEnumerable<CategoryRDTO>> GetAll()
  {
    IEnumerable<Category> list = await _context.Categories.ToListAsync();
    return list.Select(c => new CategoryRDTO { Id = c.Id, Name = c.Name, Active = c.Active });
  }
  public async Task<RepositoryResult> ActivateEntity(Guid id, bool status, string userId)
  {
    Category? dbEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category nout found")
      };
    }
    dbEntity.Activate(status, userId);
    int resultSave = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = resultSave > 0,
      Errors = null
    };
  }
}
