
using Microsoft.AspNetCore.Http.HttpResults;
using MyAPI.Entities.Categories;
using MyAPI.Entities.Products;
using System.Net.NetworkInformation;

namespace MyAPI.Entities.Orders;

public class OrderRepository : IOrderRepository
{
  private readonly AppDbContext _context;
  private readonly IMapper _rdtoMapper;
  private readonly IMapper _clientMapper;
  private readonly IMapper _productMapper;

  public OrderRepository(AppDbContext context)
  {
    _context = context;
    _clientMapper = new Mapper(new MapperConfiguration(cfg =>
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
    _rdtoMapper = new Mapper(new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<Order, OrderReportRDTO>()
      .ForMember(destination => destination.Id, options =>
          options.MapFrom(source => source.Id))
      .ForMember(destination => destination.Client, options =>
          options.MapFrom(source => _clientMapper
          .Map<ClientRDTO>(source.Client)))
      .ForMember(destination => destination.CreatedAt, options =>
          options.MapFrom(source => source.CreatedAt))
      .ForMember(destination => destination.Total, options =>
          options.MapFrom(source => source.Total));
    }));
    _productMapper = new Mapper(new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<Product, ProductRDTO>()
      .ForMember(destination => destination.Id, options =>
          options.MapFrom(source => source.Id))
      .ForMember(destination => destination.Name, options =>
          options.MapFrom(source => source.Name))
      .ForMember(destination => destination.Description, options =>
          options.MapFrom(source => source.Description))
      .ForMember(destination => destination.Category, options =>
          options.MapFrom(source => new CategoryRDTO
          {
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

  public async Task<RepositoryResult> AddEntity(OrderDTO entity, string userId)
  {
    Client? client = await _context
      .Clients
      .AsNoTracking()
      .Include(e => e.User)
      .Where(x => x.UserId == userId)
      .FirstOrDefaultAsync();

    if (client == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Client not found")
      };
    }

    if (!client.Active)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Client not active")
      };
    }

    List<Product> products = await _context
      .Products
      .Where(p => entity.Products.Contains(p.Id))
      .ToListAsync();

    if(!products.Any()) {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Products not found")
      };
    }

    Order order = new()
    {
      Id = Guid.NewGuid(),
      Client = client,
      Products = products,
      CreatedBy = userId,
      UpdatedBy = userId,
      CreatedAt = DateTime.Now,
      UpdatedAt = DateTime.Now
    };
    await _context.Orders.AddAsync(order);
    int result = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = result > 0,
      Errors = null
    };
  }

  public async Task<GetAllRDTO<OrderReportRDTO>> GetAll(
    int? limit,
    int? offset,
    Guid? clientId,
    string orderBy,
    bool desc
  ){
    var query = _context.Orders
      .AsNoTracking()
      .Include(e => e.Client)
      .Include(e => e.Client.User)
      .AsQueryable();

    if (desc == true)
    {
      if (orderBy == "client")
      {
        query.OrderByDescending(e => e.Client.Name);
      }
      else
      {
        query.OrderByDescending(e => e.CreatedAt);
      }
    }
    else
    {
      if (orderBy == "client")
      {
        query.OrderBy(e => e.Client.Name);
      }
      else
      {
        query.OrderBy(e => e.CreatedAt);
      }
    }

    if (clientId != null)
    {
      query = query.Where(r => r.Client.Id == clientId);
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

    IEnumerable<Order> list = await query
      .ToListAsync();
    return new GetAllRDTO<OrderReportRDTO>(
      count,
      _rdtoMapper.Map<IEnumerable<OrderReportRDTO>>(list)
    );
  }

  public async Task<OrderRDTO?> GetById(Guid id)
  {
    Order? entity = await _context
      .Orders
      .AsNoTracking()
      .Include(e => e.Client)
      .Include(e => e.Client.User)
      .Include(e => e.Products)
      .Where(x => x.Id == id)
      .FirstOrDefaultAsync();

    return entity != null ? new OrderRDTO(
    entity.Id,
    _clientMapper.Map<ClientRDTO>(entity.Client),
    _productMapper.Map<List<ProductRDTO>>(entity.Products)
      ) : null;
  }

  public async Task<RepositoryResult> Finish(Guid id, string userId)
  {
    Order? dbEntity = await _context
      .Orders
      .AsNoTracking()
      .Where(x => x.Id == id)
      .FirstOrDefaultAsync();

    if (dbEntity == null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Order not found")
      };
    }

    if(dbEntity.FinishedAt is not null)
    {
      return new RepositoryResult
      {
        Success = false,
        Errors = new ErrorsRDTO("Order is Already finished")
      };
    }

    dbEntity.Finish(userId);
    int result = await _context.SaveChangesAsync();

    return new RepositoryResult
    {
      Success = result > 0,
      Errors = null
    };
  }
}
