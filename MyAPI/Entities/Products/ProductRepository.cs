namespace MyAPI.Entities.Products;

public class ProductRepository : IProductRepository
{
  private readonly AppDbContext _context;
  private IMapper _rdtoMapper;
  //private IMapper _dtoMapper;

  public ProductRepository(AppDbContext context)
  {
    _context = context;
    _rdtoMapper = new Mapper(new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<Product, ProductRDTO>()
      .ForMember(destination => destination.Id, options =>
          options.MapFrom(source => source.Id))
      .ForMember(destination => destination.Name, options =>
          options.MapFrom(source => source.Name))
      .ForMember(destination => destination.Description, options =>
          options.MapFrom(source => source.Description))
      .ForMember(destination => destination.Category, options =>
          options.MapFrom(source => new CategoryRDTO {
            Id = source.Category.Id,
            Name = source.Category.Name,
            Active = source.Category.Active
          }))
      .ForMember(destination => destination.HasStock, options =>
          options.MapFrom(source => source.HasStock))
      .ForMember(destination => destination.Price, options =>
          options.MapFrom(source => source.Price))
      .ForMember(destination => destination.Active, options =>
          options.MapFrom(source => source.Active));
    }));
  }
  public async Task<RepositoryResult> AddEntity(ProductDTO entity, string userId)
  {
    Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == entity.CategoryId);
    if (category == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category nout found")
      };
    }

    if (!category.Active)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category isnt active")
      };
    }

    Product product = new()
    {
      Name = entity.Name,
      Description = entity.Description,
      Category = category,
      HasStock = entity.HasStock,
      Price = entity.Price,
      CreatedBy = userId,
      CreatedAt = DateTime.Now,
      UpdatedBy = userId,
      UpdatedAt = DateTime.Now,
    };
    await _context.Products.AddAsync(product);
    int result = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = result > 0,
      Errors = null
    };
  }

  public async Task<RepositoryResult> RemoveEntity(Guid id)
  {
    Product? dbEntity = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Product nout found")
      };
    }
    _context.Products.Remove(dbEntity);

    int resultSave = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = resultSave > 0,
      Errors = null
    };
  }
  public async Task<RepositoryResult> UpdateEntity(Guid id, ProductDTO entity, string userId)
  {
    Product? dbEntity = await _context.Products.Include(e => e.Category)
      .FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Product nout found")
      };
    }

    Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == entity.CategoryId);
    if (category == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category nout found")
      };
    }

    if (!category.Active)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Category isnt active")
      };
    }

    dbEntity.Update(
      entity.Name,
      entity.Description,
      dbEntity.Category.Id == category.Id ? dbEntity.Category : category,
      entity.HasStock,
      entity.Price,
      userId);
    int resultEmployee = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = resultEmployee > 0,
      Errors = null
    };
  }
  public async Task<ProductRDTO?> GetById(Guid id)
  {
    Product? entity = await _context
      .Products
      .AsNoTracking()
      .Include(e => e.Category)
      .Where(x => x.Id == id)
      .FirstOrDefaultAsync();

    return entity != null ? _rdtoMapper.Map<ProductRDTO>(
      entity
    ) : null;
  }
  public async Task<GetAllRDTO<ProductRDTO>> GetAll(
    int? limit,
    int? offset,
    Guid? categoryId,
    string? searchTerm,
    bool? showCase,
    string orderBy,
    bool desc
  )
  {
    var query = _context.Products
      .AsNoTracking()
      .Include(e => e.Category)
      .AsQueryable();

    if (desc == true)
    {
      if(orderBy=="name")
      {
        query.OrderByDescending(e => e.Name);
      }
      else
      {
        query.OrderByDescending(e => e.Price);
      }
    }
    else
    {
      if (orderBy == "name")
      {
        query.OrderBy(e => e.Name);
      }
      else
      {
        query.OrderBy(e => e.Price);
      }
    }

    if (showCase == true)
    {
      query = query.Where(r =>
        r.HasStock &&
        r.Active &&
        r.Category.Active
      );
    }

    if (categoryId != null)
    {
      query = query.Where(r => r.Category.Id == categoryId);
    }

    if (searchTerm != null)
    {
      query = query.Where(r => r.Name.Contains(searchTerm) || r.Description.Contains(searchTerm));
    }

    int count = await query.CountAsync();

    if (offset != null)
    {
      query = query.Skip(offset.Value);
    }

    if (limit != null)
    {
      query = query.Take(limit.Value);
    }

    IEnumerable<Product> list = await query
      .ToListAsync();
    return new GetAllRDTO<ProductRDTO>(
      count,
      _rdtoMapper.Map<IEnumerable<ProductRDTO>>(list)
    );
  }
 
  public async Task<RepositoryResult> ActivateEntity(Guid id, bool status, string userId)
  {
    Product? dbEntity = await _context.Products.FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Product nout found")
      };
    }
    dbEntity.Activate(status, userId);
    int result = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = result > 0,
      Errors = null
    };
  }

}
