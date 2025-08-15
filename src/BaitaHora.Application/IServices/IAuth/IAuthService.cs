namespace BaitaHora.Application.IServices.Auths
{
    using BaitaHora.Application.DTOs.Requests.Auth;
    using BaitaHora.Application.DTOs.Responses;
    using BaitaHora.Domain.Commons;

    public interface IAuthService
    {
        Task<Result<UserResponse>> RegisterOwnerWithCompanyAsync(RegisteOwnerRequest request, CancellationToken ct = default);
        Task<Result<UserResponse>> RegisterEmployeeAsync(RegisterEmployeeRequest request, Guid actorUserId);
        Task<Result<string>> AuthenticateAsync(LoginRequest request);
    }
}