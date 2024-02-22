namespace MyAPI.Controllers;

public enum UserType
{
  Client,
  Employee
}

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly AuthHelper _authHelper;
  private readonly ILogger<AuthController> _logger;
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IClientRepository _clientRepository;
  private readonly UserManager<IdentityUser> _userManager;

  public AuthController(
    IConfiguration config,
    ILogger<AuthController> logger,
    IClientRepository clientRepository,
    IEmployeeRepository employeeRepository,
    UserManager<IdentityUser> userManager
    )
  {
    _authHelper = new AuthHelper(config);
    _logger = logger;
    _clientRepository = clientRepository;
    _employeeRepository = employeeRepository;
    _userManager = userManager;
  }

  [AllowAnonymous]
  [HttpPost("EmployeeSingIn")]
  public async Task<IActionResult> EmployeeSingIn(SignInDTO entity)
  {
    return await SignIn(entity, UserType.Employee);    
  }
  
  [AllowAnonymous]
  [HttpPost("ClientSingIn")]
  public async Task<IActionResult> ClientSingIn(SignInDTO entity)
  {
    return await SignIn(entity, UserType.Client);    
  }

  private async Task<IActionResult> SignIn(SignInDTO entity, UserType userType)
  {
    if (!entity.IsValid)
    {
      return BadRequest(new ErrorsRDTO(entity.HandleErrors()));
    }

    IdentityUser? user = await _userManager.FindByEmailAsync(entity.Email);
    if (user == null)
    {
      return BadRequest(new ErrorsRDTO("Email or password are invalid"));
    }
    if (!_userManager.CheckPasswordAsync(user, entity.Password).Result)
    {
      return BadRequest(new ErrorsRDTO("Email or password are invalid"));
    }

    UserSignIn? userData;
    string token;
    DateTime expires = _authHelper.GetExpires(null);

    Console.WriteLine(userType);
    Console.WriteLine(UserType.Employee);
    Console.WriteLine(userType == UserType.Employee);

    if (userType == UserType.Employee)
    {
      EmployeeRDTO? employee = await _employeeRepository.GetByUserId(user.Id);

      if (employee == null || employee.Active == false)
      {
        return BadRequest(new ErrorsRDTO("Employee inactive"));
      }

      var claims = new Claim[]
      {
        new Claim("userId", user.Id),
        new Claim("userName", employee.Name),
        new Claim("employeeRegistration", employee.Registration)
      };
      
      token = _authHelper.CreateToken(claims, expires);

      userData = new UserSignIn(
        user.Id,
        employee.Name,
        userType
      );
    }
    else
    {
      ClientRDTO? client = await _clientRepository.GetByUserId(user.Id);

      if (client == null || client.Active == false)
      {
        return BadRequest(new ErrorsRDTO("Client inactive"));
      }

      var claims = new Claim[]
      {
        new Claim("userId", user.Id),
        new Claim("userName", client.Name),
        new Claim("nationalRegistrationNumber", client.NationalRegistrationNumber)
      };

      token = _authHelper.CreateToken(claims, expires);

      userData = new UserSignIn(
        user.Id,
        client.Name,
        userType
      );
    } 

    _logger.LogInformation($"user({userType}) sign in at: {DateTime.Now}");

    return Ok(new SignInRDTO(
      token,
      expires,
      userData
     )
    );
  }
}
