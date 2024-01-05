namespace MyAPI.Entities;

public class ErrorRDTO
{
  public string[] Errors { get; set; }

  public ErrorRDTO(string[] errors)
  {
    Errors = errors;
  }
}


