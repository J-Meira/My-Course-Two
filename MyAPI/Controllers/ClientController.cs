namespace MyAPI.Controllers;

[Authorize(Policy = "ClientPolicy")]
[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
  private IClientRepository _repository;

  public ClientController(IClientRepository repository)
  {
    _repository = repository;
  }

  [HttpGet("GetAll")]
  public async Task<IActionResult> GetAll(int? limit, int? offset, string? searchTerm)
  {
    GetAllRDTO<ClientRDTO> result = await _repository.GetAll(limit, offset, searchTerm);
    return Ok(result);
  }

  [HttpGet("GetById/{id:guid}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    ClientRDTO? record = await _repository.GetById(id);
    return record == null ? BadRequest() : Ok(record);
  }

  [HttpPut("UpdateById/{id:guid}")]
  public async Task<IActionResult> UpdateById(ClientUpdateDTO entity, Guid id)
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