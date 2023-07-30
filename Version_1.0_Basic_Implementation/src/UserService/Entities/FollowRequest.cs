using UserService.Entities.Enums;

namespace UserService.Entities
{
    public class FollowRequest
    {
        public int Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid RecieverId { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public RequestStatus Status { get; set; }
        public User Follower { get; set; }
        public User Followee { get; set; }
    }
}
