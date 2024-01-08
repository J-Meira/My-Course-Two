namespace MyAPI.Entities.Employees;

public interface IEmployeeRepository
{
  public RepositoryTaskResult AddEntity(EmployeeDTO entity);
  public RepositoryTaskResult UpdateEntity(Guid id, EmployeeDTO entity);
  public RepositoryTaskResult ActivateEntity(Guid id, bool status);
  public EmployeeRDTO? GetById(Guid id);
  public IEnumerable<EmployeeRDTO> GetAll(int? limit, int? offset, string? searchTerm);
}
