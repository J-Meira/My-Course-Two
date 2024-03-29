namespace MyAPI.Entities;

public class ErrorsRDTO
{
  public IEnumerable<string> Errors { get; set; }

  public ErrorsRDTO(IEnumerable<string> errors)
  {
    Errors = errors;
  }
  public ErrorsRDTO(string error)
  {
    Errors =  new List<string>() { error };
  }
}


