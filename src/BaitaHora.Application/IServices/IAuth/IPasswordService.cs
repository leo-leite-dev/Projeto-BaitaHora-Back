using BaitaHora.Application.DTOs.Requests.Auth;
using BaitaHora.Domain.Commons;

namespace BaitaHora.Application.IServices
{
    public interface IPasswordService
    {
        Task<Result<string>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<Result<string>> GeneratePasswordResetTokenAsync(string email);
        Task<Result<string>> ResetPasswordWithTokenAsync(ResetPasswordRequest request);

        string HashPassword(string rawPassword);
        bool VerifyHashedPassword(string hashedPassword, string providedPassword);
    }
}