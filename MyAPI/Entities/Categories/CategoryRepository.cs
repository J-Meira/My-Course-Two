using MyAPI.Infra.DataBase;
using System.Collections.Generic;

namespace MyAPI.Entities.Categories;

public class CategoryRepository :ICategoryRepository
{
  private readonly AppDbContext _context;
  
  public CategoryRepository(AppDbContext context)
  {
    _context = context;
  }
  public bool AddEntity(CategoryDTO entity)
  {
    Category category = new Category()
    {
      Name = entity.Name,
      CreatedBy = "Sys",
      CreatedAt = DateTime.Now,
      UpdatedBy = "Sys",
      UpdatedAt = DateTime.Now,

    };
    _context.Categories.Add(category);
    return _context.SaveChanges() > 0;
  }
  public bool RemoveEntity(Guid id)
  {
    Category? dbEntity = _context.Categories.FirstOrDefault(c => c.Id == id);
    if (dbEntity == null)
    {
      return false;
    }
    _context.Categories.Remove(dbEntity);
    return _context.SaveChanges() > 0;
  }
  public bool UpdateEntity(Guid id, CategoryDTO entity)
  {
    Category? dbEntity = _context.Categories.FirstOrDefault(c => c.Id == id);
    if (dbEntity == null)
    {
      return false;
    }
    dbEntity.Name = entity.Name;
    dbEntity.UpdatedAt = DateTime.Now;
    return _context.SaveChanges() > 0;
  }
  public CategoryRDTO? GetById(Guid id)
  {
    Category ? entity = _context.Categories.Where(x => x.Id == id).FirstOrDefault();
    return entity != null ? new CategoryRDTO()
    {
      Id = entity.Id,
      Name = entity.Name,
      Active = entity.Active,

    } : null;
  }
  public IEnumerable<CategoryRDTO> GetAll()
  {
    IEnumerable<Category> list = _context.Categories.ToList();
    return list.Select(c => new CategoryRDTO { Id = c.Id, Name = c.Name, Active = c.Active });
  }
  public bool ActiveEntity(Guid id, bool status)
  {
    Category? dbEntity = _context.Categories.FirstOrDefault(c => c.Id == id);
    if (dbEntity == null)
    {
      return false;
    }
    dbEntity.Active = status;
    dbEntity.UpdatedAt = DateTime.Now;
    return _context.SaveChanges() > 0;
  }
}
