using BaitaHora.Application.DTOs.Auth.Commands;
using BaitaHora.Application.DTOs.Commands.Auth;
using BaitaHora.Application.DTOs.Requests.Auth;
using BaitaHora.Application.DTOs.Responses;
using BaitaHora.Domain.Commons;

namespace BaitaHora.Application.IServices.Auths
{
    public interface IAuthService
    {
        Task<Result<UserResponse>> RegisterOwnerWithCompanyAsync(RegisterOwnerWithCompanyCommand cmd, CancellationToken ct = default);
        Task<Result<UserResponse>> RegisterEmployeeAsync(RegisterEmployeeCommand cmd, Guid actorUserId, CancellationToken ct = default);
        Task<Result<string>> AuthenticateAsync(LoginRequest request, CancellationToken ct = default);
    }

}