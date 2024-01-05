using Flunt.Notifications;

namespace MyAPI.Entities;

public abstract class EntityDTO : Notifiable<Notification>
{
  public Dictionary<string, string[]> HandleErrors(IReadOnlyCollection<Notification> notifications)
  {
    return notifications
      .GroupBy(g => g.Key)
      .ToDictionary(g => g.Key, g => g.Select(x => x.Message).ToArray());
  }
}
