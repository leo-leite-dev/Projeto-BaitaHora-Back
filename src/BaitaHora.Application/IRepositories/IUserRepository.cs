using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.IRepositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<bool> ExistsByEmailAsync(string email);
        Task AddUserProfileAsync(UserProfile profile);
    }
}