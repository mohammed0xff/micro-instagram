namespace Shared.Events.FollowEvents
{
    public class FollowBaseEvent : BaseEvent
    {
        public Guid FollowerId { get; set; }
        public Guid FollowedId { get; set; }
        public string FollowerUsername { get; set; }
    }
}
