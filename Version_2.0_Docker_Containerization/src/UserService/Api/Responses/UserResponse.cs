using UserService.Api.Responses.Enums;

namespace UserService.Api.Responses
{
    public class UserResponse
    {
        public string Username { get; set; }
        public bool IsPrivate { get; set; } = false;
        public string Bio { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public FollowStatus FollowStatus { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
