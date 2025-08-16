using BaitaHora.Application.DTOs.Commands.Companies;
using BaitaHora.Application.DTOs.Commands.Users;

namespace BaitaHora.Application.DTOs.Commands.Auth
{
    public sealed record RegisterOwnerWithCompanyCommand(
        UserInput User,
        CreateCompanyCommand Company
    );
}