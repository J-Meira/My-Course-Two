namespace MyAPI.Entities;

public class RepositoryTaskResult
{
  public bool Success { get; set; }
  public ErrorsRDTO? Errors { get; set; }
}
