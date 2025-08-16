using BaitaHora.Application.DTOs.Auth.Commands;
using BaitaHora.Application.DTOs.Users.Responses;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Domain.Commons;
using MediatR;

namespace BaitaHora.Application.DTOs.Auth.Handler
{
    public sealed class RegisterOwnerWithCompanyHandler
        : IRequestHandler<RegisterOwnerWithCompanyCommand, Result<UserResponse>>
    {
        private readonly IAuthService _auth;
        public RegisterOwnerWithCompanyHandler(IAuthService auth) => _auth = auth;

        public Task<Result<UserResponse>> Handle(
            RegisterOwnerWithCompanyCommand cmd, CancellationToken ct)
            => _auth.RegisterOwnerWithCompanyAsync(cmd, ct);
    }
}