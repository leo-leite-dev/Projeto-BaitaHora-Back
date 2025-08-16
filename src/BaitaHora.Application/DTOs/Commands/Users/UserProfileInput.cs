using BaitaHora.Application.DTOs.Commands.Commons;

namespace BaitaHora.Application.DTOs.Commands.Users
{
    public sealed record UserProfileInput(
        string FullName,
        string Cpf,
        string Rg,
        DateTime BirthDate,
        string Phone,
        AddressInput Address,
        string? ProfileImageUrl
    );
}