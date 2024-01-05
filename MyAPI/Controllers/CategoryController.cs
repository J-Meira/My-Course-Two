using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

  [HttpGet("GetById/{id}")]
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
      return BadRequest(entity.HandleErrors(entity.Notifications));
    }
    return _categoryRepository.AddEntity(entity) ? Created() : BadRequest();
  }

  [HttpPut("UpdateById/{id}")]
  public IActionResult UpdateById(CategoryDTO entity, Guid id)
  {
    return _categoryRepository.UpdateEntity(id, entity) ? Ok() : BadRequest();
  }

  [HttpPut("Active/{id}/{status}")]
  public IActionResult Active(Guid id, bool status)
  {
    return _categoryRepository.ActiveEntity(id, status) ? Ok() : BadRequest();
  }

  [HttpDelete("DeleteById/{id}")]
  public IActionResult DeleteById(Guid id)
  {
    return _categoryRepository.RemoveEntity(id) ? Ok() : BadRequest();
  }
}
