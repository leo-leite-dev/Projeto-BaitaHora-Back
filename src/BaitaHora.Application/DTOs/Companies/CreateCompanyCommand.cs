using BaitaHora.Application.DTOs.Commands.Commons;

namespace BaitaHora.Application.DTOs.Commands.Companies
{
    public sealed record CreateCompanyCommand(
        string Name,
        string? Document,
        string? ImageUrl,
        AddressDto Address
    );

    
}