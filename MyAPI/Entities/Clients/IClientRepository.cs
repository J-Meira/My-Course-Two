namespace MyAPI.Entities.Clients;

public interface IClientRepository
{
  public Task<RepositoryResult> AddEntity(ClientDTO entity, string userId);
  public Task<RepositoryResult> UpdateEntity(Guid id, ClientUpdateDTO entity, string userId);
  public Task<RepositoryResult> ActivateEntity(Guid id, bool status, string userId);
  public Task<ClientRDTO?> GetById(Guid id);
  public Task<ClientRDTO?> GetByUserId(string userId);
  public Task<GetAllRDTO<ClientRDTO>> GetAll(int? limit, int? offset, string? searchTerm);
}
