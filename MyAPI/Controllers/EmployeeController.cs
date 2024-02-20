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
  public async Task<IActionResult> GetAll(int? limit, int? offset, string? searchTerm)
  {
    IEnumerable<EmployeeRDTO> result = await _repository.GetAll(limit, offset, searchTerm);
    return Ok(new GetAllRDTO<EmployeeRDTO>(result.Count(), result));
  }

  [HttpGet("GetById/{id:guid}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    EmployeeRDTO? record = await _repository.GetById(id);
    return record == null ? BadRequest() : Ok(record);
  }

  //[AllowAnonymous]
  [HttpPost("Add")]
  public async Task<IActionResult> Add(EmployeeDTO entity)
  {
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
      //userId = "sys";
    }
    if (!entity.IsValid)
    {
      return BadRequest(new ErrorsRDTO(entity.HandleErrors()));
    }
    RepositoryResult result = await _repository.AddEntity(entity, userId);
    return result.Success ? Created() : BadRequest(result.Errors);
  }

  [HttpPut("UpdateById/{id:guid}")]
  public async Task<IActionResult> UpdateById(EmployeeUpdateDTO entity, Guid id)
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

}
