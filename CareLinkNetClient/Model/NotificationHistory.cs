namespace CareLinkNetClient.Model;

public class NotificationHistory
{
    public List<ActiveNotification> ActiveNotifications = new();
    public List<ClearedNotification> ClearedNotifications = new();
}