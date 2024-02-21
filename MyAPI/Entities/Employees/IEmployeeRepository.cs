namespace MyAPI.Entities.Employees;

public interface IEmployeeRepository
{
  public Task<RepositoryResult> AddEntity(EmployeeDTO entity, string userId);
  public Task<RepositoryResult> UpdateEntity(Guid id, EmployeeUpdateDTO entity, string userId);
  public Task<RepositoryResult> ActivateEntity(Guid id, bool status, string userId);
  public Task<EmployeeRDTO?> GetById(Guid id);
  public Task<EmployeeRDTO?> GetByUserId(string userId);
  public Task<GetAllRDTO<EmployeeRDTO>> GetAll(int? limit, int? offset, string? searchTerm);
}
