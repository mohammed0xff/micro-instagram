namespace UserService.Api.Requests
{
    public class LoginRequest
    {
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
    }
}
