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

  [HttpGet("GetAll")]
  public async Task<IActionResult> GetAll()
  {
    GetAllRDTO<CategoryRDTO> result = await _repository.GetAll(false);
    return Ok(result);
  }

  [HttpGet("GetById/{id:guid}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    CategoryRDTO ? category =  await _repository.GetById(id);
    return category == null ? BadRequest() : Ok(category);
  }

  [HttpPost("Add")]
  public async Task<IActionResult> Add(CategoryDTO entity)
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
    RepositoryResult result = await _repository.AddEntity(entity, userId);
    return  result.Success? Created() : BadRequest(result.Errors);
  }

  [HttpPut("UpdateById/{id:guid}")]
  public async Task<IActionResult> UpdateById(CategoryDTO entity, Guid id)
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
    RepositoryResult result = await _repository.UpdateEntity(id, entity, userId);
    return result.Success ? Ok() : BadRequest(result.Errors);
  }

  [HttpPut("Activate/{id:guid}/{status:bool}")]
  public async Task<IActionResult> Activate(Guid id, bool status)
  {
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }
    RepositoryResult result = await _repository.ActivateEntity(id, status, userId);
    return result.Success ? Ok() : BadRequest(result.Errors);

  }

  [HttpDelete("DeleteById/{id:guid}")]
  public async Task<IActionResult> DeleteById(Guid id)
  {
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }
    RepositoryResult result = await _repository.RemoveEntity(id);
    return result.Success ? Ok() : BadRequest(result.Errors);
  }
}
