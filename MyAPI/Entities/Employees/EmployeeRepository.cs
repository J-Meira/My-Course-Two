namespace MyAPI.Entities.Employees;

public class EmployeeRepository : IEmployeeRepository
{
  private readonly AppDbContext _context;
  private UserManager<IdentityUser> _userManager;
  private IMapper _rdtoMapper;
  //private IMapper _dtoMapper;
  private IdentityHelper _identityHelper;

  public EmployeeRepository(AppDbContext context, UserManager<IdentityUser> userManager)
  {
    _context = context;
    _identityHelper = new IdentityHelper();
    _userManager = userManager;
    _rdtoMapper = new Mapper(new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<Employee, EmployeeRDTO>()
      .ForMember(destination => destination.Id, options =>
          options.MapFrom(source => source.Id))
      .ForMember(destination => destination.Name, options =>
          options.MapFrom(source => source.Name))
      .ForMember(destination => destination.Email, options =>
          options.MapFrom(source => source.User.Email))
      .ForMember(destination => destination.Registration, options =>
          options.MapFrom(source => source.Registration))
      .ForMember(destination => destination.Active, options =>
          options.MapFrom(source => source.Active));
    }));
  }
  public async Task<RepositoryResult> AddEntity(EmployeeDTO entity, string userId)
  {
    IdentityUser user = new IdentityUser
    {
      UserName = entity.Email,
      Email = entity.Email,
    };

    IdentityResult result = await _userManager.CreateAsync(user, entity.Password);

    if (!result.Succeeded)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = _identityHelper.ArrangeErrors(result.Errors),
      };      
    }

    Employee employee = new ()
    {
      Name = entity.Name,
      Registration = entity.Registration,
      User = user,
      CreatedBy = userId,
      CreatedAt = DateTime.Now,
      UpdatedBy = userId,
      UpdatedAt = DateTime.Now,
    };

    await _context.Employees.AddAsync(employee);
    int resultEmployee = await _context.SaveChangesAsync();

    return new RepositoryResult {
      Success= resultEmployee > 0,
      Errors= null
    };
  }

  public async Task<RepositoryResult> UpdateEntity(Guid id, EmployeeUpdateDTO entity, string userId)
  {
    Employee? dbEntity = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Employee not found")
      };
    }
    if(dbEntity.User.Email != entity.Email)
    {
      //update email
      IdentityResult setEmailResult = await _userManager.SetEmailAsync(dbEntity.User, entity.Email);
      if (!setEmailResult.Succeeded)
      {
        return new RepositoryResult
        {
          Success = false,
          Errors = _identityHelper.ArrangeErrors(setEmailResult.Errors),
        };
      }

      IdentityResult setUserNameResult = await _userManager.SetUserNameAsync(dbEntity.User, entity.Email);
      if (!setUserNameResult.Succeeded)
      {
        return new RepositoryResult
        {
          Success = false,
          Errors = _identityHelper.ArrangeErrors(setUserNameResult.Errors),
        };
      }

    }
    dbEntity.Update(entity.Name, entity.Registration, userId);
    int resultEmployee = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = resultEmployee > 0,
      Errors = null
    };
  }
  public async Task<EmployeeRDTO?> GetById(Guid id)
  {
    Employee? entity = await _context
      .Employees
      .AsNoTracking()
      .Include(e=>e.User)
      .Where(x => x.Id == id)
      .FirstOrDefaultAsync();

    return entity != null ? _rdtoMapper.Map<EmployeeRDTO>(
      entity
    ) : null;
  }
  public async Task<EmployeeRDTO?> GetByUserId(string userId)
  {
    Employee? entity = await _context
      .Employees
      .AsNoTracking()
      .Include(e=>e.User)
      .Where(x => x.UserId == userId)
      .FirstOrDefaultAsync();

    return entity != null ? _rdtoMapper.Map<EmployeeRDTO>(
      entity
    ) : null;
  }
  public async Task<GetAllRDTO<EmployeeRDTO>> GetAll(int? limit, int? offset, string? searchTerm)
  {
    var query = _context.Employees
      .AsNoTracking()
      .Include(e => e.User)
      .OrderBy(r => r.Name)
      .AsQueryable();

    if (searchTerm != null)
    {
      query = query.Where(r=> r.Name.Contains(searchTerm) || r.User.Email.Contains(searchTerm));
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

    IEnumerable<Employee> list = await query
      .ToListAsync();

    return new GetAllRDTO<EmployeeRDTO>(
      count,
      _rdtoMapper.Map<IEnumerable<EmployeeRDTO>>(list)
    );
  }
  public async Task<RepositoryResult> ActivateEntity(Guid id, bool status, string userId)
  {
    Employee? dbEntity = await _context.Employees.FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Employee not found")
      };
    }
    dbEntity.Activate(status, userId);
    int resultEmployee = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = resultEmployee > 0,
      Errors = null
    };
  }
}
