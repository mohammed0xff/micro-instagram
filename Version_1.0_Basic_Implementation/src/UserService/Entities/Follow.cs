namespace UserService.Entities
{
    public class Follow
    {
        public Guid Id { get; set; }
        public Guid FollowerId { get; set; }
        public Guid FolloweeId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public User Follower { get; set; }
        public User Followee { get; set; }
        public DateTime FollowDate { get; set; } = DateTime.Now;
    }
}
