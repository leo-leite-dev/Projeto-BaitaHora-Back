namespace BaitaHora.Application.DTOs.Auth.Requests
{
    public sealed class LoginRequest
    {
        public string Identifier { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
