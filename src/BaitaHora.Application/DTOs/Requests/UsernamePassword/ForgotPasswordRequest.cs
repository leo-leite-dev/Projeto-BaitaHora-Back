namespace BaitaHora.Application.DTOs.Requests.Auth
{
    public sealed class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}