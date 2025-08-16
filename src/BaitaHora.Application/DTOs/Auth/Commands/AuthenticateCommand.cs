using MediatR;
using BaitaHora.Domain.Commons;

namespace BaitaHora.Application.DTOs.Auth.Commands
{
    public sealed record AuthenticateCommand(
        string UsernameOrEmail,
        string Password
    ) : IRequest<Result<string>>;
}
