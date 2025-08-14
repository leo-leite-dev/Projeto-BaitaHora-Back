namespace BaitaHora.Application.DTOs.Requests
{
    public sealed class AddressRequest
    {
        public string Street { get; init; } = string.Empty;
        public string Number { get; init; } = string.Empty;
        public string? Complement { get; init; }
        public string District { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string State { get; init; } = string.Empty;
        public string ZipCode { get; init; } = string.Empty; 
    }
}