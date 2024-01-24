namespace MyAPI.Entities;

public class RepositoryResult
{
  public bool Success { get; set; }
  public ErrorsRDTO? Errors { get; set; }
}
