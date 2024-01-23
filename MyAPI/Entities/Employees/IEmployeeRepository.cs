namespace MyAPI.Entities.Employees;

public interface IEmployeeRepository
{
  public RepositoryTaskResult AddEntity(EmployeeDTO entity, string userId);
  public RepositoryTaskResult UpdateEntity(Guid id, EmployeeUpdateDTO entity, string userId);
  public RepositoryTaskResult ActivateEntity(Guid id, bool status, string userId);
  public EmployeeRDTO? GetById(Guid id);
  public EmployeeRDTO? GetByUserId(string userId);
  public IEnumerable<EmployeeRDTO> GetAll(int? limit, int? offset, string? searchTerm);
}
