namespace BaitaHora.Application.DTOs.Requests
{
    public sealed class UserRequest
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string? Username { get; set; }

        public UserProfileRequest Profile { get; init; } = new();
    }
}