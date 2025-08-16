using BaitaHora.Domain.Entities.Scheduling;

namespace BaitaHora.Application.IRepositories
{
    public interface IScheduleRepository : IGenericRepository<Schedule>
    {
        Task<Schedule?> GetByUserAsync(Guid userId, Guid companyId, CancellationToken ct = default);
        Task<bool> ExistsForUserAsync(Guid userId, Guid companyId, CancellationToken ct = default);
        Task<Schedule?> GetWithAppointmentsAsync(Guid userId, Guid companyId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
    }
}