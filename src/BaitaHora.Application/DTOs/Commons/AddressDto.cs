namespace BaitaHora.Application.DTOs.Commands.Commons
{
    public sealed record AddressDto(
        string Street,
        string Number,
        string District,
        string City,
        string State,
        string ZipCode,
        string? Complement
    );
}