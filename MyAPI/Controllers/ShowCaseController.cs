using MyAPI.Entities.Categories;

namespace MyAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShowCaseController : ControllerBase
{
  private ICategoryRepository _categoryRepository;
  private IProductRepository _productRepository;

  public ShowCaseController(
    ICategoryRepository categoryRepository,
    IProductRepository productRepository
  )
  {
    _categoryRepository = categoryRepository;
    _productRepository = productRepository;
  }

  [HttpGet("GetProducts")]
  public async Task<IActionResult> ShowCase(
    int? limit,
    int? offset,
    Guid? categoryId,
    string? searchTerm,
    string orderBy = "name",
    bool desc = false
  )
  {
    if (limit > 100)
    {
      return BadRequest(new ErrorsRDTO("Limit should be less or equals than 100"));
    }
    if (orderBy != "name" && orderBy != "price")
    {
      return BadRequest(new ErrorsRDTO("Order only is posible by 'name' or 'price'"));
    }
    GetAllRDTO<ProductRDTO> result = await _productRepository
      .GetAll(limit, offset, categoryId, searchTerm, true, orderBy, desc);
    return Ok(result);
  }

  [HttpGet("GetCategories")]
  public async Task<IActionResult> GetShowCaseCategories()
  {
    GetAllRDTO<CategoryRDTO> result = await _categoryRepository.GetAll(true);
    return Ok(result);
  }
}
