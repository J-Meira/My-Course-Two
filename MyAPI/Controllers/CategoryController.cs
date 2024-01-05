using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Entities;
using MyAPI.Entities.Categories;

namespace MyAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
  private ICategoryRepository _categoryRepository;

  public CategoryController(ICategoryRepository categoryRepository)
  {
    _categoryRepository = categoryRepository;
  }

  [HttpGet("GetAll")]
  public IEnumerable<CategoryRDTO> GetAll()
  {
    return _categoryRepository.GetAll();
  }

  [HttpGet("GetById/{id:guid}")]
  public IActionResult GetById(Guid id)
  {
    CategoryRDTO ? category =  _categoryRepository.GetById(id);
    return category == null ? BadRequest() : Ok(category);
  }

  [HttpPost("Add")]
  public IActionResult Add(CategoryDTO entity)
  {
    if(!entity.IsValid)
    {
      return BadRequest(new ErrorRDTO(entity.HandleErrors()));
    }
    return _categoryRepository.AddEntity(entity) ? Created() : BadRequest();
  }

  [HttpPut("UpdateById/{id:guid}")]
  public IActionResult UpdateById(CategoryDTO entity, Guid id)
  {
    if(!entity.IsValid)
    {
      return BadRequest(entity.HandleErrors());
    }
    return _categoryRepository.UpdateEntity(id, entity) ? Ok() : BadRequest();
  }

  [HttpPut("Activate/{id:guid}/{status:bool}")]
  public IActionResult Activate(Guid id, bool status)
  {
    return _categoryRepository.ActivateEntity(id, status) ? Ok() : BadRequest();
  }

  [HttpDelete("DeleteById/{id:guid}")]
  public IActionResult DeleteById(Guid id)
  {
    return _categoryRepository.RemoveEntity(id) ? Ok() : BadRequest();
  }
}
