using NotificationService.Data;
using NotificationService.Models;
using Shared.Events;
using Shared.Events.FollowEvents.Follow;

namespace NotificationService.EventHandling
{
    public class FollowEventHandler : IEventHandler<FollowCreatedEvent>
    {
        private readonly INotificationRepositroy _notificationRepository;

        public FollowEventHandler(INotificationRepositroy notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task HandleAsync(FollowCreatedEvent @event)
        {
            // save a notification for the followed user
            var notification = new Notification
            {
                SenderId = @event.FollowerId,
                ReceiverId = @event.FollowedId,
                Message = $"User {@event.FollowerUsername} followed you.",
            };

            await _notificationRepository.SaveNotificationAsync(notification);
            
            // TODO : if user is connected send them in real time 
        }
    }
}
