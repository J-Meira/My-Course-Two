using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Entities;
using MyAPI.Entities.Employees;

namespace MyAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
  private UserManager<IdentityUser> _userManager;
  private IEmployeeRepository _repository;

  public EmployeeController(UserManager<IdentityUser> userManager, IEmployeeRepository repository)
  {
    _userManager = userManager;
    _repository = repository;
  }

  [HttpGet("GetAll")]
  public IEnumerable<EmployeeRDTO> GetAll(int? limit, int? offset, string? searchTerm)
  {
    return _repository.GetAll(limit, offset, searchTerm);
  }

  [HttpGet("GetById/{id:guid}")]
  public IActionResult GetById(Guid id)
  {
    EmployeeRDTO? record = _repository.GetById(id);
    return record == null ? BadRequest() : Ok(record);
  }

  [HttpPost("Add")]
  public IActionResult Add(EmployeeDTO entity)
  {
    if(!entity.IsValid)
    {
      return BadRequest(new ErrorsRDTO(entity.HandleErrors()));
    }

    var result = _repository.AddEntity(entity);

    return result.Success ? Created() : BadRequest(result.Errors);
  }

  [HttpPut("UpdateById/{id:guid}")]
  public IActionResult UpdateById(EmployeeDTO entity, Guid id)
  {
    if (!entity.IsValid)
    {
      return BadRequest(entity.HandleErrors());
    }
    var result = _repository.UpdateEntity(id, entity);
    return result.Success ? Ok() : BadRequest(result.Errors);
  }

  [HttpPut("Activate/{id:guid}/{status:bool}")]
  public IActionResult Activate(Guid id, bool status)
  {
    var result = _repository.ActivateEntity(id, status);
    return result.Success ? Ok() : BadRequest(result.Errors);
  }

}
