using Shared.Events;

namespace NotificationService.EventProcessing
{
    public interface IEventProcessor
    {
        Task ProcessEvent(BaseEvent @event);
    }
}
