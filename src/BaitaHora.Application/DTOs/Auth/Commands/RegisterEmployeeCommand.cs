using MediatR;
using BaitaHora.Domain.Commons;
using BaitaHora.Application.DTOs.Commands.Users;
using BaitaHora.Domain.Enums;
using BaitaHora.Application.DTOs.Users.Responses;

namespace BaitaHora.Application.DTOs.Auth.Commands
{
    public sealed record RegisterEmployeeCommand : IRequest<Result<UserResponse>>
    {
        public Guid ActorUserId { get; init; }
        public Guid CompanyId { get; init; }
        public UserInput User { get; init; } = default!;
        public CompanyRole Role { get; init; }
    }
}