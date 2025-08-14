namespace BaitaHora.Application.DTOs.Requests.Auth
{
    public sealed class LoginRequest
    {
        public string Identifier { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
