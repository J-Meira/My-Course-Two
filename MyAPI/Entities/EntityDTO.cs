namespace MyAPI.Entities;

public abstract class EntityDTO : Notifiable<Notification>
{  
  public string[] HandleErrors()
  {
    return Notifications.Select(x => x.Message).ToArray();
  }
}
