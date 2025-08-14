namespace BaitaHora.Application.DTOs.Responses
{
    public sealed record AddressResponse
    {
        public string Street { get; init; } = string.Empty;
        public string Number { get; init; } = string.Empty;
        public string District { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string ZipCode { get; init; } = string.Empty;
        public string? Complement { get; init; }
    }
}