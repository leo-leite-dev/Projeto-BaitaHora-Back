using BaitaHora.Application.DTOs.Requests.Auth;
using BaitaHora.Application.DTOs.Responses;
using BaitaHora.Domain.Commons;

namespace BaitaHora.Application.IServices.IAuth
{
    public interface IAuthService
    {
        Task<Result<UserResponse>> RegisterEmployeeAsync(RegisterEmployeeRequest request, Guid actorUserId);
        Task<Result<UserResponse>> RegisterOwnerWithCompanyAsync(RegisteOwnerRequest request);
        Task<Result<string>> AuthenticateAsync(LoginRequest request);
    }
}