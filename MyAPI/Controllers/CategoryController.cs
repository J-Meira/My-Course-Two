using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Entities;
using MyAPI.Entities.Categories;
using System.Security.Claims;

namespace MyAPI.Controllers;

[Authorize]
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
    if (int.TryParse(this.User
        .FindFirstValue("userId"), out int userId))
    {
      if (!entity.IsValid)
      {
        return BadRequest(new ErrorsRDTO(entity.HandleErrors()));
      }
      return _repository.AddEntity(entity) ? Created() : BadRequest();
    }
    else
    {
      return BadRequest();
    }
  }

  [HttpPut("UpdateById/{id:guid}")]
  public IActionResult UpdateById(CategoryDTO entity, Guid id)
  {
    if(!entity.IsValid)
    {
      return BadRequest(entity.HandleErrors());
    }
    return _repository.UpdateEntity(id, entity) ? Ok() : BadRequest();
  }

  [HttpPut("Activate/{id:guid}/{status:bool}")]
  public IActionResult Activate(Guid id, bool status)
  {
    return _repository.ActivateEntity(id, status) ? Ok() : BadRequest();
  }

  [HttpDelete("DeleteById/{id:guid}")]
  public IActionResult DeleteById(Guid id)
  {
    return _repository.RemoveEntity(id) ? Ok() : BadRequest();
  }
}
