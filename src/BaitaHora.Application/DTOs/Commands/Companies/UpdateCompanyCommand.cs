using BaitaHora.Application.DTOs.Commands.Commons;

namespace BaitaHora.Application.DTOs.Commands.Companies
{
    public sealed record UpdateCompanyCommand(
        Guid CompanyId,
        Guid RequesterUserId,
        string? Name,
        string? Document,
        string? ImageUrl,
        AddressInput? Address
    );
}