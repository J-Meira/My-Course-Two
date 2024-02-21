namespace MyAPI.Entities.Clients;

public class ClientRepository : IClientRepository
{
  private readonly AppDbContext _context;
  private UserManager<IdentityUser> _userManager;
  private IMapper _rdtoMapper;
  //private IMapper _dtoMapper;
  private IdentityHelper _identityHelper;

  public ClientRepository(AppDbContext context, UserManager<IdentityUser> userManager)
  {
    _context = context;
    _identityHelper = new IdentityHelper();
    _userManager = userManager;
    _rdtoMapper = new Mapper(new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<Client, ClientRDTO>()
      .ForMember(destination => destination.Id, options =>
          options.MapFrom(source => source.Id))
      .ForMember(destination => destination.Name, options =>
          options.MapFrom(source => source.Name))
      .ForMember(destination => destination.Email, options =>
          options.MapFrom(source => source.User.Email))
      .ForMember(destination => destination.NationalRegistrationNumber, options =>
          options.MapFrom(source => source.NationalRegistrationNumber))
      .ForMember(destination => destination.Active, options =>
          options.MapFrom(source => source.Active));
    }));
  }
  public async Task<RepositoryResult> AddEntity(ClientDTO entity, string userId)
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

    Client client = new ()
    {
      Name = entity.Name,
      NationalRegistrationNumber = entity.NationalRegistrationNumber,
      User = user,
      CreatedBy = userId,
      CreatedAt = DateTime.Now,
      UpdatedBy = userId,
      UpdatedAt = DateTime.Now,
    };

    await _context.Clients.AddAsync(client);
    int resultEmployee = await _context.SaveChangesAsync();

    return new RepositoryResult {
      Success= resultEmployee > 0,
      Errors= null
    };
  }

  public async Task<RepositoryResult> UpdateEntity(Guid id, ClientUpdateDTO entity, string userId)
  {
    Client? dbEntity = await _context.Clients.Include(e => e.User).FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Client nout found")
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
    dbEntity.Update(entity.Name, entity.NationalRegistrationNumber, userId);
    int resultClient = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = resultClient > 0,
      Errors = null
    };
  }
  public async Task<ClientRDTO?> GetById(Guid id)
  {
    Client? entity = await _context
      .Clients
      .AsNoTracking()
      .Include(e=>e.User)
      .Where(x => x.Id == id)
      .FirstOrDefaultAsync();

    return entity != null ? _rdtoMapper.Map<ClientRDTO>(
      entity
    ) : null;
  }
  public async Task<ClientRDTO?> GetByUserId(string userId)
  {
    Client? entity = await _context
      .Clients
      .AsNoTracking()
      .Include(e=>e.User)
      .Where(x => x.UserId == userId)
      .FirstOrDefaultAsync();

    return entity != null ? _rdtoMapper.Map<ClientRDTO>(
      entity
    ) : null;
  }
  public async Task<GetAllRDTO<ClientRDTO>> GetAll(int? limit, int? offset, string? searchTerm)
  {
    var query = _context.Clients
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

    IEnumerable<Client> list = await query
      .ToListAsync();

    return new GetAllRDTO<ClientRDTO>(
      count,
      _rdtoMapper.Map<IEnumerable<ClientRDTO>>(list)
    );
  }
  public async Task<RepositoryResult> ActivateEntity(Guid id, bool status, string userId)
  {
    Client? dbEntity = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Client nout found")
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
