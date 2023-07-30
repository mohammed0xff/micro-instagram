namespace Shared.Events
{
    public abstract class BaseEvent
    {
        public BaseEvent()
        {
        }

        public Guid Id { get; } = Guid.NewGuid();
        public DateTime CreationDate { get; } = DateTime.Now;
    }

    public interface IEventHandler<in TEvent> where TEvent : BaseEvent
    {
        Task HandleAsync(TEvent @event);
    }
}
