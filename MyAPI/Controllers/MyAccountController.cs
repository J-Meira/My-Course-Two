namespace MyAPI.Controllers;

[Authorize(Policy = "ClientPolicy")]
[ApiController]
[Route("api/[controller]")]
public class MyAccountController : ControllerBase
{
  private readonly IClientRepository _clientRepository;
  private readonly IOrderRepository _orderRepository;

  public MyAccountController(
    IClientRepository clientRepository,
    IOrderRepository orderRepository
  )
  {
    _clientRepository = clientRepository;
    _orderRepository = orderRepository;
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

  [HttpPost("CreateOrder")]
  public async Task<IActionResult> CreateOrder(OrderDTO entity)
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
    RepositoryResult result = await _orderRepository.AddEntity(entity, userId);
    return result.Success ? Created() : BadRequest(result.Errors);
  }

  [HttpGet("GetOrders")]
  public async Task<IActionResult> GetOrders(
    int? limit,
    int? offset,
    string orderBy = "createdAt",
    bool desc = true
  ){
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }

    ClientRDTO? client = await _clientRepository.GetByUserId(userId);

    if (client is null)
    {
      return BadRequest(new ErrorsRDTO("Client Not Found"));
    }

    if (limit > 100)
    {
      return BadRequest(new ErrorsRDTO("Limit should be less or equals than 100"));
    }
    if (orderBy != "client" && orderBy != "createdAt")
    {
      return BadRequest(new ErrorsRDTO("Order only is posible by 'client' or 'createdAt'"));
    }

    GetAllRDTO<OrderReportRDTO> result = await _orderRepository
      .GetAll(limit, offset, client.Id, orderBy, desc);

    return Ok(result);
  }

  [HttpGet("GetOrdersById/{id:guid}")]
  public async Task<IActionResult> GetOrdersById(Guid id)
  {
    string? userId = this.User.FindFirstValue("userId");
    if (userId is null)
    {
      return BadRequest(new ErrorsRDTO("User Not Found"));
    }

    ClientRDTO? client = await _clientRepository.GetByUserId(userId);

    if (client is null)
    {
      return BadRequest(new ErrorsRDTO("Client Not Found"));
    }

    OrderRDTO? order = await _orderRepository.GetById(id);

    return order == null || order.Client.Id != client.Id ? BadRequest() : Ok(order);
  }

}
