namespace BaitaHora.Application.DTOs.Users.Responses
{
    public sealed record UserResponse
    {
        public Guid Id { get; init; }
        public string? Username { get; init; }
        public string Email { get; init; } = string.Empty;

        public bool IsActive { get; init; }
        public DateTimeOffset CreatedAtUtc { get; init; }
        public DateTimeOffset? UpdatedAtUtc { get; init; }

        public UserProfileResponse Profile { get; init; } = new();
    }
}