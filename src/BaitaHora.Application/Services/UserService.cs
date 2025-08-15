using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices;
using BaitaHora.Domain.Entities;
using BaitaHora.Domain.Factories;

namespace BaitaHora.Application.Services
{
    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _users;
        private readonly IPasswordService _passwords;

        public UserService(IUserRepository users, IPasswordService passwords)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _passwords = passwords ?? throw new ArgumentNullException(nameof(passwords));
        }

        public async Task<User> CreateOwnerUserAsync(string email, string rawPassword, string username, UserProfile profile, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("E-mail obrigatório.", nameof(email));
            if (string.IsNullOrWhiteSpace(rawPassword)) throw new ArgumentException("Senha obrigatória.", nameof(rawPassword));
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username obrigatório.", nameof(username));
            if (profile is null) throw new ArgumentNullException(nameof(profile));

            var user = UserFactory.Create(
                email: email.Trim(),
                rawPassword: rawPassword,
                profile: profile,
                username: username.Trim(),
                hashFunction: _passwords.HashPassword
            );

            await _users.AddAsync(user);
            return user;
        }
    }
}
