using BaitaHora.Application.DTOs.Auth.Commands;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Domain.Commons;
using MediatR;

namespace BaitaHora.Application.DTOs.Auth.Handler
{
    public sealed class AuthenticateHandler
        : IRequestHandler<AuthenticateCommand, Result<string>>
    {
        private readonly IAuthService _auth;
        public AuthenticateHandler(IAuthService auth) => _auth = auth;

        public Task<Result<string>> Handle(AuthenticateCommand cmd, CancellationToken ct)
            => _auth.AuthenticateAsync(cmd, ct);
    }
}