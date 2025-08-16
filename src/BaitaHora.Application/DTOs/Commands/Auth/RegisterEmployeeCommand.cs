using BaitaHora.Application.DTOs.Commands.Users;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.DTOs.Auth.Commands
{
    public sealed record RegisterEmployeeCommand(
        Guid CompanyId,
        UserInput User,
        CompanyRole Role
    );
}