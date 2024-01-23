using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Entities;
using MyAPI.Entities.Employees;
using System.Security.Claims;

namespace MyAPI.Controllers;

[Authorize(Policy = "EmployeePolicy")]
[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
  private IEmployeeRepository _repository;

  public EmployeeController(IEmployeeRepository repository)
  {
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
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }
    if (!entity.IsValid)
    {
      return BadRequest(new ErrorsRDTO(entity.HandleErrors()));
    }
    RepositoryTaskResult result = _repository.AddEntity(entity, userId);
    return result.Success ? Created() : BadRequest(result.Errors);
  }

  [HttpPut("UpdateById/{id:guid}")]
  public IActionResult UpdateById(EmployeeUpdateDTO entity, Guid id)
  {
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }
    if (!entity.IsValid)
    {
      return BadRequest(entity.HandleErrors());
    }
    RepositoryTaskResult result = _repository.UpdateEntity(id, entity, userId);
    return result.Success ? Ok() : BadRequest(result.Errors);
  }

  [HttpPut("Activate/{id:guid}/{status:bool}")]
  public IActionResult Activate(Guid id, bool status)
  {
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }
    RepositoryTaskResult result = _repository.ActivateEntity(id, status, userId);
    return result.Success ? Ok() : BadRequest(result.Errors);
  }

}
