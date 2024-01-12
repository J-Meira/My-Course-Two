using Microsoft.AspNetCore.Mvc;
using MyAPI.Entities.Employees;
using MyAPI.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using MyAPI.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace MyAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
  private UserManager<IdentityUser> _userManager;
  private IEmployeeRepository _repository;
  private readonly AuthHelper _authHelper;

  public AuthController(UserManager<IdentityUser> userManager, IEmployeeRepository repository, IConfiguration config)
  {
    _userManager = userManager;
    _repository = repository;
    _authHelper = new AuthHelper(config);
  }

  [AllowAnonymous]
  [HttpPost("SingIn")]
  public IActionResult Add(SignInDTO entity)
  {
    if (!entity.IsValid)
    {
      return BadRequest(new ErrorsRDTO(entity.HandleErrors()));
    }

    IdentityUser? user = _userManager.FindByEmailAsync(entity.Email).Result;
    if (user == null)
    {
      return BadRequest(new ErrorsRDTO(new List<string>(["Email or password are invalid"])));
    }
    if (!_userManager.CheckPasswordAsync(user, entity.Password).Result)
    {
      return BadRequest(new ErrorsRDTO(new List<string>(["Email or password are invalid"])));
    }

    EmployeeRDTO? employee = _repository.GetByUserId(user.Id);

    if (employee == null || employee.Active == false)
    {
      return BadRequest(new ErrorsRDTO(new List<string>(["User inactive"])));
    }

    var claims = new Claim[]
    {
      new Claim("userId", user.Id),
      new Claim("userName", employee.Name),
      new Claim("userRegistration", employee.Registration),
    };
    DateTime expires = DateTime.UtcNow.AddMinutes(2);

    string token = _authHelper
      .CreateToken(claims,expires);

    return Ok(new
    {
      token = token,
      expires = expires
    });
  }
}
