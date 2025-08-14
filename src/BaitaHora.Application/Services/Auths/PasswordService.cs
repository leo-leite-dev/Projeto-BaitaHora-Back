using Microsoft.Extensions.Logging;
using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Commons;
using BaitaHora.Domain.Exceptions;
using BaitaHora.Domain.Entities;
using BaitaHora.Application.DTOs.Requests.Auth;
using BaitaHora.Application.IServices;
using BaitaHora.Application.IServices.Auth;

public class PasswordService : IPasswordService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordManager _passwordManager;
    private readonly IEmailService _emailService;
    private readonly ILogger<PasswordService> _logger;

    public PasswordService(
        IUserRepository userRepository,
        IPasswordManager passwordManager,
        IEmailService emailService,
        ILogger<PasswordService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordManager = passwordManager ?? throw new ArgumentNullException(nameof(passwordManager));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<string>> ChangePasswordAsync(ChangePasswordRequest request)
    {
        try
        {
            var user = await GetUserByIdentifierAsync(request.Username);
            if (user is null)
                return Result<string>.Failure("Usuário não encontrado.");

            user.ChangePassword(
                currentRawPassword: request.CurrentPassword,
                newRawPassword: request.NewPassword,
                verifyPassword: raw => _passwordManager.Verify(raw, user.PasswordHash),
                hashFunction: _passwordManager.Hash
            );

            await _userRepository.UpdateAsync(user);

            return Result<string>.Success("Senha alterada com sucesso.");
        }
        catch (UserException ex)
        {
            _logger.LogWarning(ex, "Erro ao trocar senha.");
            return Result<string>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao trocar senha.");
            return Result<string>.Failure("Erro interno ao trocar senha.");
        }
    }

    public async Task<Result<string>> GeneratePasswordResetTokenAsync(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user is null)
                return Result<string>.Failure("Usuário não encontrado.");

            user.GeneratePasswordResetToken(() => Guid.NewGuid().ToString(), TimeSpan.FromHours(1));
            await _userRepository.UpdateAsync(user);

            await _emailService.SendAsync(
                toEmail: user.Email,
                subject: "Redefinição de Senha - BaitaHora",
                body: $@"
                <p>Olá, {user.Username}</p>
                <p>Foi solicitado um processo de redefinição de senha para sua conta.</p>
                <p>Use o token abaixo no sistema para redefinir sua senha. Este token é válido por 1 hora.</p>
                <p><strong>Token:</strong> {user.PasswordResetToken}</p>
                <p>Se você não solicitou isso, ignore este e-mail.</p>"
            );

            return Result<string>.Success("Token gerado e enviado por e-mail.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar token de recuperação de senha.");
            return Result<string>.Failure("Erro interno ao gerar token.");
        }
    }

    public async Task<Result<string>> ResetPasswordWithTokenAsync(ResetPasswordRequest request)
    {
        try
        {
            var user = await GetUserByIdentifierAsync(request.Username);
            if (user is null)
                return Result<string>.Failure("Usuário não encontrado.");

            user.ResetPasswordWithToken(request.Token, _passwordManager.Hash(request.NewPassword));
            await _userRepository.UpdateAsync(user);

            return Result<string>.Success("Senha redefinida com sucesso.");
        }
        catch (UserException ex)
        {
            _logger.LogWarning(ex, "Erro ao redefinir senha.");
            return Result<string>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao redefinir senha.");
            return Result<string>.Failure("Erro interno ao redefinir senha.");
        }
    }

    private async Task<User?> GetUserByIdentifierAsync(string identifier)
    {
        return identifier.Contains("@")
            ? await _userRepository.GetByEmailAsync(identifier)
            : await _userRepository.GetByUsernameAsync(identifier);
    }
}