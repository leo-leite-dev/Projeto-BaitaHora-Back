using BaitaHora.Domain.Entities.Commons;
using BaitaHora.Domain.Exceptions;
using BaitaHora.Domain.Validators;

namespace BaitaHora.Domain.Entities.Users
{
    public class User : Base
    {
        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;

        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        public string? PasswordResetToken { get; private set; }
        public DateTime? PasswordResetTokenExpiresAt { get; private set; }

        public Guid ProfileId { get; private set; }
        public UserProfile Profile { get; private set; } = null!;

        private User() { }

        private User(string email, UserProfile profile, string username)
        {
            if (profile is null)
                throw new UserException("Perfil do usuário é obrigatório.");

            Profile = profile;
            ProfileId = profile.Id;

            Initialize(email, username);
        }

        private void Initialize(string email, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new UserException("O username é obrigatório.");
                
            if (string.IsNullOrWhiteSpace(email))
                throw new UserException("O e-mail é obrigatório.");

            Email = EmailValidator.EnsureValid(email.Trim());
            IsActive = true;
            CreatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(username))
                SetUsername(username.Trim(), _ => false);
        }

        public static User Create(string email, string rawPassword, UserProfile profile, string username,
            Func<string, string> hashFunction,
            Func<string, bool>? isEmailTaken = null,
            Func<string, bool>? isUsernameTaken = null)
        {
            if (hashFunction is null)
                throw new ArgumentNullException(nameof(hashFunction));

            var user = new User(email, profile, username);

            if (isEmailTaken is not null)
                user.SetEmail(user.Email, isEmailTaken);

            if (!string.IsNullOrWhiteSpace(username) && isUsernameTaken is not null)
                user.SetUsername(user.Username!, isUsernameTaken);

            user.SetPassword(rawPassword, hashFunction);

            return user;
        }

        public void SetEmail(string newEmail, Func<string, bool> isEmailTaken)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new UserException("O e-mail é obrigatório.");

            var normalized = EmailValidator.EnsureValid(newEmail.Trim());

            if (string.Equals(Email, normalized, StringComparison.OrdinalIgnoreCase))
                return;

            if (isEmailTaken.Invoke(normalized))
                throw new UserException("E-mail já está em uso.");

            Email = normalized;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetUsername(string newUsername, Func<string, bool> isUsernameTaken)
        {
            if (string.IsNullOrWhiteSpace(newUsername))
                throw new UserException("O nome de usuário não pode estar vazio.");

            newUsername = newUsername.Trim();

            if (newUsername.Contains(' '))
                throw new UserException("O nome de usuário não pode conter espaços.");

            if (newUsername.Length < 4 || newUsername.Length > 20)
                throw new UserException("O nome de usuário deve ter entre 4 e 20 caracteres.");

            if (isUsernameTaken.Invoke(newUsername))
                throw new UserException("Este nome de usuário já está em uso.");

            Username = newUsername;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetPassword(string rawPassword, Func<string, string> hashFunction)
        {
            if (hashFunction is null)
                throw new ArgumentNullException(nameof(hashFunction));

            PasswordValidator.EnsureValid(rawPassword);
            PasswordHash = hashFunction.Invoke(rawPassword);
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangePassword(string currentRawPassword, string newRawPassword,
            Func<string, bool> verifyPassword,
            Func<string, string> hashFunction)
        {
            if (verifyPassword is null) throw new ArgumentNullException(nameof(verifyPassword));
            if (hashFunction is null) throw new ArgumentNullException(nameof(hashFunction));

            if (!verifyPassword.Invoke(currentRawPassword))
                throw new UserException("A senha atual está incorreta.");

            PasswordValidator.EnsureValid(newRawPassword);

            var newHash = hashFunction.Invoke(newRawPassword);
            if (newHash == PasswordHash)
                throw new UserException("A nova senha deve ser diferente da senha atual.");

            PasswordHash = newHash;
            UpdatedAt = DateTime.UtcNow;
        }

        public void GeneratePasswordResetToken(Func<string> tokenGenerator, TimeSpan duration)
        {
            if (tokenGenerator is null) throw new ArgumentNullException(nameof(tokenGenerator));
            if (duration <= TimeSpan.Zero) throw new UserException("Duração inválida para o token de recuperação.");

            PasswordResetToken = tokenGenerator.Invoke();
            PasswordResetTokenExpiresAt = DateTime.UtcNow.Add(duration);
        }

        public void ResetPasswordWithToken(string token, string newRawPassword, Func<string, string> hashFunction)
        {
            if (hashFunction is null) throw new ArgumentNullException(nameof(hashFunction));

            if (PasswordResetToken == null || PasswordResetTokenExpiresAt == null)
                throw new UserException("Nenhuma solicitação de recuperação de senha foi feita.");

            if (PasswordResetTokenExpiresAt < DateTime.UtcNow)
                throw new UserException("O token de recuperação de senha expirou.");

            if (!string.Equals(PasswordResetToken, token, StringComparison.Ordinal))
                throw new UserException("Token de recuperação de senha inválido.");

            SetPassword(newRawPassword, hashFunction);

            PasswordResetToken = null;
            PasswordResetTokenExpiresAt = null;
        }

        public void Activate()
        {
            if (!IsActive)
            {
                IsActive = true;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Remove() => Deactivate();
    }
}