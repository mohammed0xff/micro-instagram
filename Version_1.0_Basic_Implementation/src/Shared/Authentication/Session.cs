using Microsoft.AspNetCore.Http;
using Shared.Authentication;
using System;
using System.Linq;
using System.Security.Claims;

public class Session : IUserSession
{
    public Guid UserId { get; }
    public string Username { get; }
    public string Email { get; }
    public bool IsAuthenticated { get; } = false;

    private readonly ClaimsPrincipal _user;

    public Session(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        _user = user ?? throw new ArgumentNullException(nameof(user));

        IsAuthenticated = _user.Identity?.IsAuthenticated ?? false;

        if (IsAuthenticated)
        {
            UserId = GetUserId();
            Email = GetClaimValue(ClaimTypes.Email);
            Username = GetClaimValue("username");
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = _user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
        {
            throw new InvalidOperationException($"Could not get user ID from claim {ClaimTypes.NameIdentifier}.");
        }

        return userId;
    }

    private string GetClaimValue(string claimType)
    {
        return _user.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
    }
}