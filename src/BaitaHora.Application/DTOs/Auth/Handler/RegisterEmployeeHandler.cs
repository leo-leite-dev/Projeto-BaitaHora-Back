using MediatR;
using BaitaHora.Domain.Commons;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.DTOs.Auth.Commands;
using BaitaHora.Application.DTOs.Users.Responses;

namespace BaitaHora.Application.DTOs.Auth.Handler
{
    public sealed class RegisterEmployeeHandler
        : IRequestHandler<RegisterEmployeeCommand, Result<UserResponse>>
    {
        private readonly IAuthService _auth;
        public RegisterEmployeeHandler(IAuthService auth) => _auth = auth;

        public Task<Result<UserResponse>> Handle(RegisterEmployeeCommand cmd, CancellationToken ct)
            => _auth.RegisterEmployeeAsync(cmd, ct);
    }
}