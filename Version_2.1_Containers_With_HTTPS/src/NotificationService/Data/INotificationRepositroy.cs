using NotificationService.DBContext;
using NotificationService.Models;

namespace NotificationService.Data
{
    public interface INotificationRepositroy
    {
        Task SaveNotificationAsync(Notification notification);
        Task<List<Notification>> GetRecentNotificationsAsync(Guid receiverId, int pageNumber, int pageSize);
        Task ReadRecentNotificationsAsync(Guid receiverId);
        Task DeleteNotificationAsync(Notification notification);
    }
}