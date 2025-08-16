using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Auth;
using BaitaHora.Application.IServices.Users;
using BaitaHora.Domain.Entities.Users;
using BaitaHora.Domain.Exceptions;

namespace BaitaHora.Application.Services.Users
{
    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordService _passwords;
        private readonly IUnitOfWork _uow;

        public UserService(
            IUserRepository users,
            IPasswordService passwords,
            IUnitOfWork uow)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _passwords = passwords ?? throw new ArgumentNullException(nameof(passwords));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<User> CreateOwnerUserAsync(string email, string rawPassword, string username, UserProfile profile, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("E-mail obrigatório.", nameof(email));
            if (string.IsNullOrWhiteSpace(rawPassword)) throw new ArgumentException("Senha obrigatória.", nameof(rawPassword));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username obrigatório.", nameof(username));
            if (profile is null) throw new ArgumentNullException(nameof(profile));

            var emailNorm = email.Trim();
            var userNameNorm = username.Trim();

            if (await _users.ExistsByEmailAsync(emailNorm, ct))
                throw new UserException("E-mail já está em uso.");

            if (await _users.ExistsByUsernameAsync(userNameNorm, ct))
                throw new UserException("Este nome de usuário já está em uso.");

            var user = User.Create(
                email: emailNorm,
                rawPassword: rawPassword,
                profile: profile,
                username: userNameNorm,
                hashFunction: _passwords.HashPassword
            );

            await _users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            return user;
        }

    }
}
