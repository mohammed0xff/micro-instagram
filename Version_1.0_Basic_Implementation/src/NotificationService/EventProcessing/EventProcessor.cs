using Shared.Events;

namespace NotificationService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public EventProcessor(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }
        public async Task ProcessEvent(BaseEvent @event)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var handlers = scope.ServiceProvider
                    .GetServices(typeof(IEventHandler<>)
                    .MakeGenericType(@event.GetType()));

                foreach (var handler in handlers)
                {
                    await ((dynamic)handler).HandleAsync((dynamic)@event);
                }
            }
        }
    }
}
