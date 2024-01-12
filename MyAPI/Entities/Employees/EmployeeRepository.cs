using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyAPI.Infra.DataBase;
using System.Collections.Generic;

namespace MyAPI.Entities.Employees;

public class EmployeeRepository : IEmployeeRepository
{
  private readonly AppDbContext _context;
  private UserManager<IdentityUser> _userManager;
  private IMapper _rdtoMapper;
  //private IMapper _dtoMapper;

  public EmployeeRepository(AppDbContext context, UserManager<IdentityUser> userManager)
  {
    _context = context;
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
  public RepositoryTaskResult AddEntity(EmployeeDTO entity, string userId)
  {
    IdentityUser user = new IdentityUser
    {
      UserName = entity.Email,
      Email = entity.Email,
    };

    IdentityResult result = _userManager.CreateAsync(user, entity.Password).Result;

    if (!result.Succeeded)
    {
      return new RepositoryTaskResult
      {
        Success = false,
        Errors = ArrangeIdentityErrors(result.Errors),
      };      
    }

    Employee Employee = new ()
    {
      Name = entity.Name,
      Registration = entity.Registration,
      User = user,
      CreatedBy = userId,
      CreatedAt = DateTime.Now,
      UpdatedBy = userId,
      UpdatedAt = DateTime.Now,
    };
    _context.Employees.Add(Employee);
    int resultEmployee = _context.SaveChanges();

    return new RepositoryTaskResult {
      Success= resultEmployee > 0,
      Errors= null
    };
  }

  public RepositoryTaskResult UpdateEntity(Guid id, EmployeeDTO entity, string userId)
  {
    Employee? dbEntity = _context.Employees.Include(e => e.User).FirstOrDefault(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryTaskResult
      {
        Success = false,
        Errors = new ErrorsRDTO(new List<string>(["Employee nout found"]))
      };
    }
    if(dbEntity.User.Email != entity.Email)
    {
      //update email
      IdentityResult setEmailResult = _userManager.SetEmailAsync(dbEntity.User, entity.Email).Result;
      if (!setEmailResult.Succeeded)
      {
        return new RepositoryTaskResult
        {
          Success = false,
          Errors = ArrangeIdentityErrors(setEmailResult.Errors),
        };
      }

    }
    dbEntity.Update(entity.Name, entity.Registration, userId);
    int resultEmployee = _context.SaveChanges();

    return new RepositoryTaskResult
    {
      Success = resultEmployee > 0,
      Errors = null
    };
  }
  public EmployeeRDTO? GetById(Guid id)
  {
    Employee? entity = _context.Employees
      .Include(e=>e.User)
      .Where(x => x.Id == id).FirstOrDefault();
    return entity != null ? _rdtoMapper.Map<EmployeeRDTO>(
      entity
    ) : null;
  }
  public EmployeeRDTO? GetByUserId(string userId)
  {
    Employee? entity = _context.Employees
      .Include(e=>e.User)
      .Where(x => x.UserId == userId).FirstOrDefault();
    return entity != null ? _rdtoMapper.Map<EmployeeRDTO>(
      entity
    ) : null;
  }
  public IEnumerable<EmployeeRDTO> GetAll(int? limit, int? offset, string? searchTerm)
  {
    var query = _context.Employees
      .Include(e => e.User)
      .OrderBy(r => r.Name)
      .AsQueryable();

    if (searchTerm != null)
    {
      query = query.Where(r=> r.Name.Contains(searchTerm) || r.User.Email.Contains(searchTerm));
    }

    if (offset != null)
    {
      query = query.Skip(offset.Value);
    }

    if (limit != null)
    {
      query = query.Take(limit.Value);
    }    


    IEnumerable<Employee> list = query
      .ToList();
    return _rdtoMapper.Map<IEnumerable<EmployeeRDTO>>(list);
  }
  public RepositoryTaskResult ActivateEntity(Guid id, bool status, string userId)
  {
    Employee? dbEntity = _context.Employees.FirstOrDefault(c => c.Id == id);
    if (dbEntity == null)
    {
      return new RepositoryTaskResult
      {
        Success = false,
        Errors = new ErrorsRDTO(new List<string>(["Employee nout found"]))
      };
    }
    dbEntity.Activate(status, userId);
    int resultEmployee = _context.SaveChanges();

    return new RepositoryTaskResult
    {
      Success = resultEmployee > 0,
      Errors = null
    };
  }

  private ErrorsRDTO ArrangeIdentityErrors(IEnumerable<IdentityError> errors)
  {
    List<string> list = new List<string>();
    foreach (var error in errors)
    {
      list.Add(error.Description);
    }
    return new ErrorsRDTO(list);
  }
}
