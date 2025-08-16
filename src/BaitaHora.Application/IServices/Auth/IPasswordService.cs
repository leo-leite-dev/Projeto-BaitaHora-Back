using BaitaHora.Application.DTOs.Requests.Auth;
using BaitaHora.Domain.Commons;

namespace BaitaHora.Application.IServices.Auth
{
    public interface IPasswordService
    {
        Task<Result<string>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default);
        Task<Result<string>> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default);
        Task<Result<string>> ResetPasswordWithTokenAsync(ResetPasswordRequest request, CancellationToken ct = default);

        string HashPassword(string rawPassword);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}