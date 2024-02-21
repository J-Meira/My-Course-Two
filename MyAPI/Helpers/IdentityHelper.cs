namespace MyAPI.Helpers;

public class IdentityHelper()
{
  public ErrorsRDTO ArrangeErrors(IEnumerable<IdentityError> errors)
  {
    List<string> list = new List<string>();
    foreach (var error in errors)
    {
      list.Add(error.Description);
    }
    return new ErrorsRDTO(list);
  }
}
