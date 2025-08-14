namespace BaitaHora.Application.DTOs.Requests
{
    public sealed class UserProfileRequest
    {
        public string FullName { get; init; } = string.Empty;
        public string CPF { get; init; } = string.Empty;
        public string? RG { get; init; }
        public DateTime? BirthDate { get; init; }
        public string? Phone { get; init; }
        public AddressRequest Address { get; init; } = new();
        public string? ProfileImageUrl { get; init; }
    }
}