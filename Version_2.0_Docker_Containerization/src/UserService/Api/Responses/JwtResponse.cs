namespace UserService.Api.Responses
{
    public record JwtResponse
    {
        public string Token { get; init; } = null!;
        public DateTime ExpDate { get; init; }
    }
}
