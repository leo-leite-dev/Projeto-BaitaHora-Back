using BaitaHora.Domain.Entities.Users;

namespace BaitaHora.Application.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

        Task<User?> GetByPhoneAsync(string phoneE164, CancellationToken ct = default);

        Task<string> GenerateUsernameAsync(string fullName, CancellationToken ct = default);
    }
}