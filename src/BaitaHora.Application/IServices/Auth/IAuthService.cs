using BaitaHora.Domain.Commons;
using BaitaHora.Application.DTOs.Auth.Commands;
using BaitaHora.Application.DTOs.Users.Responses;

namespace BaitaHora.Application.IServices.Auth
{
    public interface IAuthService
    {
        Task<Result<UserResponse>> RegisterOwnerWithCompanyAsync(RegisterOwnerWithCompanyCommand cmd, CancellationToken ct = default);
        Task<Result<UserResponse>> RegisterEmployeeAsync(RegisterEmployeeCommand cmd, CancellationToken ct = default);
        Task<Result<string>> AuthenticateAsync(AuthenticateCommand cmd, CancellationToken ct = default);
    }
}