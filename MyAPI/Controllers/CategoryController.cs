using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Entities;
using MyAPI.Entities.Categories;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace MyAPI.Controllers;

[Authorize(Policy = "EmployeePolicy")]
[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
  private ICategoryRepository _repository;

  public CategoryController(ICategoryRepository repository)
  {
    _repository = repository;
  }

  [AllowAnonymous]
  [HttpGet("GetAll")]
  public IEnumerable<CategoryRDTO> GetAll()
  {
    return _repository.GetAll();
  }

  [HttpGet("GetById/{id:guid}")]
  public IActionResult GetById(Guid id)
  {
    CategoryRDTO ? category =  _repository.GetById(id);
    return category == null ? BadRequest() : Ok(category);
  }

  [HttpPost("Add")]
  public IActionResult Add(CategoryDTO entity)
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
    return  result.Success? Created() : BadRequest(result.Errors);
  }

  [HttpPut("UpdateById/{id:guid}")]
  public IActionResult UpdateById(CategoryDTO entity, Guid id)
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

  [HttpDelete("DeleteById/{id:guid}")]
  public IActionResult DeleteById(Guid id)
  {
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }
    RepositoryTaskResult result = _repository.RemoveEntity(id);
    return result.Success ? Ok() : BadRequest(result.Errors);
  }
}
