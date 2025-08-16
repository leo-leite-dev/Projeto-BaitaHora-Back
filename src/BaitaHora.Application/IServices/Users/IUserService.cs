using BaitaHora.Domain.Entities.Users;

namespace BaitaHora.Application.IServices.Users
{
    public interface IUserService
    {
        Task<User> CreateOwnerUserAsync(string email, string rawPassword, string username, UserProfile profile, CancellationToken ct = default);
    }
}