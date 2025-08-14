namespace BaitaHora.Application.DTOs.Requests.Company
{
    public sealed class CompanyRequest
    {
        public string Name { get; init; } = string.Empty;
        public string? Document { get; init; }
        public string? ImageUrl { get; init; }

        public AddressRequest Address { get; init; } = new();
    }
}