namespace MyAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private readonly AuthHelper _authHelper;
  private readonly ILogger<AuthController> _logger;
  private IEmployeeRepository _repository;
  private UserManager<IdentityUser> _userManager;

  public AuthController(
    IConfiguration config,
    ILogger<AuthController> logger,
    IEmployeeRepository repository,
    UserManager<IdentityUser> userManager
    )
  {
    _authHelper = new AuthHelper(config);
    _logger = logger;
    _repository = repository;
    _userManager = userManager;
  }

  [AllowAnonymous]
  [HttpPost("SingIn")]
  public async Task<IActionResult> SingIn(SignInDTO entity)
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

    EmployeeRDTO? employee = await _repository.GetByUserId(user.Id);

    if (employee == null || employee.Active == false)
    {
      return BadRequest(new ErrorsRDTO("User inactive"));
    }

    var claims = new Claim[]
    {
      new Claim("userId", user.Id),
      new Claim("userName", employee.Name),
      new Claim("employeeRegistration", employee.Registration),
    };
    DateTime expires = DateTime.UtcNow.AddDays(1);

    string token = _authHelper
      .CreateToken(claims,expires);

    _logger.LogInformation($"user sign in at: {DateTime.Now}");

    return Ok(new SignInRDTO(
      token,
      expires,
      new UserSignIn(
        user.Id,
        employee.Name,
        employee.Registration      
      )
     )
    );
  }
}
