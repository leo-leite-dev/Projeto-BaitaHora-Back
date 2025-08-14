namespace BaitaHora.Application.DTOs.Responses
{
    public sealed record UserProfileResponse
    {
        public Guid Id { get; init; }
        public string FullName { get; init; } = string.Empty;

        public string? CpfMasked { get; init; }

        public string? Rg { get; init; }
        public DateTimeOffset? BirthDateUtc { get; init; }
        public string? Phone { get; init; }

        public AddressResponse Address { get; init; } = new();
        public string? ProfileImageUrl { get; init; }
    }
}