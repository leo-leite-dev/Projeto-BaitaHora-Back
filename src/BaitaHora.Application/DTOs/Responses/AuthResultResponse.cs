namespace BaitaHora.Application.DTOs.Responses
{
    public sealed class AuthResponse
    {
        public string Token { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
    }
}