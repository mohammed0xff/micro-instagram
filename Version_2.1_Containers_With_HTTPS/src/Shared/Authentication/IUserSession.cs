namespace Shared.Authentication
{
    public interface IUserSession
    {
        Guid UserId { get; }
        string Username { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
    }
}
