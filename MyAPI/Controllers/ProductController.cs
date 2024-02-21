namespace MyAPI.Controllers;

[Authorize(Policy = "EmployeePolicy")]
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
  private IProductRepository _repository;

  public ProductController(IProductRepository repository)
  {
    _repository = repository;
  }

  [HttpGet("GetAll")]
  public async Task<IActionResult> GetAll(
    int? limit,
    int? offset,
    Guid? categoryId,
    string? searchTerm,
    string orderBy = "name",
    bool desc = false
  )
  {
    if(limit > 100)
    {
      return BadRequest(new ErrorsRDTO("Limit should be less or equals than 100"));
    }
    if(orderBy != "name" && orderBy != "price")
    {
      return BadRequest(new ErrorsRDTO("Order only is posible by 'name' or 'price'"));
    }
    GetAllRDTO<ProductRDTO> result = await _repository
      .GetAll(limit, offset, categoryId, searchTerm, true, orderBy, desc);
    return Ok(result);
  }

  [HttpGet("GetById/{id:guid}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    ProductRDTO ? product =  await _repository.GetById(id);
    return product == null ? BadRequest() : Ok(product);
  }

  [HttpPost("Add")]
  public async Task<IActionResult> Add(ProductDTO entity)
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
  public async Task<IActionResult> UpdateById(ProductDTO entity, Guid id)
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
