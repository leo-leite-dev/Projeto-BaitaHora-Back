using BaitaHora.Domain.Entities.Commons;

namespace BaitaHora.Application.IRepositories
{
    public interface IGenericRepository<T> where T : Base
    {
        Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<T>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(T entity, CancellationToken ct = default);
        Task UpdateAsync(T entity, CancellationToken ct = default);
        Task DeleteAsync(T entity, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    }
}