namespace MyAPI.Entities.Categories;

public class CategoryRepository : ICategoryRepository
{
  private readonly AppDbContext _context;
  private IMapper _rdtoMapper;

  public CategoryRepository(AppDbContext context)
  {
    _context = context;
    _rdtoMapper = new Mapper(new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<Category, CategoryRDTO>()
      .ForMember(destination => destination.Id, options =>
          options.MapFrom(source => source.Id))
      .ForMember(destination => destination.Name, options =>
          options.MapFrom(source => source.Name))
      .ForMember(destination => destination.Active, options =>
          options.MapFrom(source => source.Active));
    }));
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
        Errors = new ErrorsRDTO("Category not found")
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
        Errors = new ErrorsRDTO("Category not found")
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
    Category? entity = await _context
      .Categories
      .AsNoTracking()
      .Where(x => x.Id == id)
      .FirstOrDefaultAsync();

    return entity != null ? new CategoryRDTO()
    {
      Id = entity.Id,
      Name = entity.Name,
      Active = entity.Active,

    } : null;
  }
  public async Task<GetAllRDTO<CategoryRDTO>> GetAll(bool showCase)
  {
    var query = _context.Categories
      .AsNoTracking()
      .OrderBy(r => r.Name)
      .AsQueryable();

    if (showCase == true)
    {
      query = query.Where(r => r.Active);
    }

    IEnumerable<Category> list = await query.ToListAsync();

    return new GetAllRDTO<CategoryRDTO>(
      list.Count(),
      _rdtoMapper.Map<IEnumerable<CategoryRDTO>>(list)
    );    
  }

  public async Task<RepositoryResult> ActivateEntity(Guid id, bool status, string userId)
  {
    Category? dbEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category not found")
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
