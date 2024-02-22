namespace MyAPI.Controllers;

[Authorize(Policy = "EmployeePolicy")]
[ApiController]
[Route("api/[controller]")]
public class OrderController : Controller
{
  private readonly IOrderRepository _repository;

  public OrderController(IOrderRepository repository)
  {
    _repository = repository;
  }

  [HttpGet("GetAll")]
  public async Task<IActionResult> GetAll(
    int? limit,
    int? offset,
    Guid? clientId,
    string orderBy = "createdAt",
    bool desc = true
  ){
    if (limit > 100)
    {
      return BadRequest(new ErrorsRDTO("Limit should be less or equals than 100"));
    }
    if (orderBy != "client" && orderBy != "createdAt")
    {
      return BadRequest(new ErrorsRDTO("Order only is posible by 'client' or 'createdAt'"));
    }

    GetAllRDTO<OrderReportRDTO> result = await _repository
      .GetAll(limit, offset, clientId, orderBy, desc);

    return Ok(result);
  }

  [HttpGet("GetById/{id:guid}")]
  public async Task<IActionResult> GetById(Guid id)
  {
    OrderRDTO? order = await _repository.GetById(id);
    return order == null ? BadRequest() : Ok(order);
  }

  [HttpPut("FinishById/{id:guid}")]
  public async Task<IActionResult> FinishId(Guid id)
  {
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }

    RepositoryResult result = await _repository.Finish(id, userId);
    return result.Success ? Ok() : BadRequest(result.Errors);
  }
}
