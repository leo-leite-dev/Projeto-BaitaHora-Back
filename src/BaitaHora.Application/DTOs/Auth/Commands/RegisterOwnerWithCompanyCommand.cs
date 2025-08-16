using BaitaHora.Application.DTOs.Commands.Companies;
using BaitaHora.Application.DTOs.Commands.Users;
using BaitaHora.Application.DTOs.Users.Responses;
using BaitaHora.Domain.Commons;
using MediatR;

namespace BaitaHora.Application.DTOs.Auth.Commands
{
    public sealed record RegisterOwnerWithCompanyCommand(
        UserInput User,
        CreateCompanyCommand Company
    ) : IRequest<Result<UserResponse>>;
}