using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Entities;
using MyAPI.Entities.Employees;
using System.Security.Claims;

namespace MyAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
  private UserManager<IdentityUser> _userManager;

  public EmployeeController(UserManager<IdentityUser> userManager)
  {
    _userManager = userManager;
  }

  [HttpGet("GetAll")]
  public IEnumerable<EmployeeRDTO> GetAll()
  {
    IEnumerable<IdentityUser> list = _userManager.Users.ToList();
    List<EmployeeRDTO> valueReturn = new List<EmployeeRDTO>();
    foreach (IdentityUser user in list)
    {
      IEnumerable<Claim>? claims = _userManager.GetClaimsAsync(user).Result;
      string? userName = claims?.FirstOrDefault(c => c.Type == "Name")?.Value??null;
      string? userRegistration = claims?.FirstOrDefault(c => c.Type == "Registration")?.Value ?? null;
      valueReturn.Add(new EmployeeRDTO(
        user.Id,
        userName,
        user.Email,
        userRegistration
        ));
    }
    return valueReturn;
  }

  //[HttpGet("GetById/{id:guid}")]
  //public IActionResult GetById(Guid id)
  //{
  //  CategoryRDTO ? category =  _categoryRepository.GetById(id);
  //  return category == null ? BadRequest() : Ok(category);
  //}

  [HttpPost("Add")]
  public IActionResult Add(EmployeeDTO entity)
  {
    if(!entity.IsValid)
    {
      return BadRequest(new ErrorRDTO(entity.HandleErrors()));
    }

    IdentityUser user = new IdentityUser {
      UserName = entity.Email,
      Email = entity.Email,
    };

    IdentityResult result = _userManager.CreateAsync(user, entity.Password).Result;

    if(!result.Succeeded)
    {
      return BadRequest(result.Errors);
    }

    IEnumerable<Claim> userClaims = new List<Claim>
    {
      new Claim("Registration", entity.Registration),
      new Claim("Name", entity.Name),
    };

    IdentityResult resultClaims = _userManager.AddClaimsAsync(user, userClaims).Result;

    if (!resultClaims.Succeeded)
    {
      return BadRequest(result.Errors);
    }
    return Created();
  }

  //[HttpPut("UpdateById/{id:guid}")]
  //public IActionResult UpdateById(CategoryDTO entity, Guid id)
  //{
  //  if(!entity.IsValid)
  //  {
  //    return BadRequest(entity.HandleErrors());
  //  }
  //  return _categoryRepository.UpdateEntity(id, entity) ? Ok() : BadRequest();
  //}

  //[HttpPut("Activate/{id:guid}/{status:bool}")]
  //public IActionResult Activate(Guid id, bool status)
  //{
  //  return _categoryRepository.ActivateEntity(id, status) ? Ok() : BadRequest();
  //}

  //[HttpDelete("DeleteById/{id:guid}")]
  //public IActionResult DeleteById(Guid id)
  //{
  //  return _categoryRepository.RemoveEntity(id) ? Ok() : BadRequest();
  //}
}
