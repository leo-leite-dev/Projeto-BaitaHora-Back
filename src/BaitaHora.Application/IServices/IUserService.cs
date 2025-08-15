using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.IServices
{
    public interface IUserService
    {
        Task<User> CreateOwnerUserAsync(string email, string rawPassword, string username, UserProfile profile, CancellationToken ct = default);
    }
}