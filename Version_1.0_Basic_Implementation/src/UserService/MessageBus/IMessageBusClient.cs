using Shared.Events;

namespace UserService.MessageBus
{
    public interface IMessageBusClient
    {
        void PublishFollowEvent(BaseEvent @event);
        void PublishUserEvent(BaseEvent @event);
    }
}
