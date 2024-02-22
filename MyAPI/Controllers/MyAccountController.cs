namespace MyAPI.Controllers;

[Authorize(Policy = "ClientPolicy")]
[ApiController]
[Route("api/[controller]")]
public class MyAccountController : ControllerBase
{
  private IClientRepository _clientRepository;

  public MyAccountController(IClientRepository clientRepository)
  {
    _clientRepository = clientRepository;
  }

  [AllowAnonymous]
  [HttpPost("Create")]
  public async Task<IActionResult> Create(ClientDTO entity)
  {
    if (!entity.IsValid)
    {
      return BadRequest(new ErrorsRDTO(entity.HandleErrors()));
    }
    RepositoryResult result = await _clientRepository.AddEntity(entity, "sys");
    return result.Success ? Created() : BadRequest(result.Errors);
  }

  [HttpGet("GetData")]
  public async Task<IActionResult> GetData()
  {
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }
    ClientRDTO? record = await _clientRepository.GetByUserId(userId);
    return record == null ? BadRequest() : Ok(record);
  }

  [HttpPut("Update")]
  public async Task<IActionResult> UpdateById(ClientUpdateDTO entity)
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
    ClientRDTO? record = await _clientRepository.GetByUserId(userId);
    if (record is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }
    RepositoryResult result = await _clientRepository.UpdateEntity(record.Id, entity, userId);
    return result.Success ? Ok() : BadRequest(result.Errors);
  }

}
