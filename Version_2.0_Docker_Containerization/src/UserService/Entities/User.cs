namespace UserService.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public bool IsPrivate { get; set; } = false;
        public string Bio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Password { get; set; }
        public ICollection<Follow> Followers { get; set; }
        public ICollection<Follow> Following { get; set; }
        public ICollection<FollowRequest> FollowRequestsReceived { get; set; }
        public ICollection<FollowRequest> FollowRequestsSent { get; set; }
    }
}
