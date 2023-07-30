using Microsoft.EntityFrameworkCore;
using NotificationService.DBContext;
using NotificationService.Models;

namespace NotificationService.Data
{
    public class NotificationRepositroy : INotificationRepositroy
    {
        private readonly AppDbContext _dbContext;
        public NotificationRepositroy(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Notification>> GetRecentNotificationsAsync(
            Guid receiverId, int pageNumber, int pageSize
            )
        {
            var newNotifications = await _dbContext.Notifications
                .AsNoTracking()
                .Where(x => x.ReceiverId == receiverId)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return newNotifications;
        }

        public async Task ReadRecentNotificationsAsync(Guid receiverId)
        {
            await _dbContext.Notifications
                .Where(x => x.ReceiverId == receiverId && x.ReadAt == null)
                .ExecuteUpdateAsync(x => x.SetProperty(x => x.ReadAt, v => DateTime.Now));

            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveNotificationAsync(Notification notification)
        {
            await _dbContext.Notifications.AddAsync(notification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(Notification notification)
        {
            _dbContext.Notifications.Remove(notification);
            await _dbContext.SaveChangesAsync();
        }
    }
}
