using GameStore.Domain.Entities.Enums;

namespace GameStore.BLL.DTOs.Notifications;

public class NotificationsList
{
    public List<UserNotificationTypes> Notifications { get; set; }
}