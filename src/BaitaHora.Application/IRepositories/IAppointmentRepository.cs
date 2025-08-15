using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.IRepositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IReadOnlyList<Appointment>> GetByScheduleAsync(Guid scheduleId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default);
        Task<bool> HasConflictAsync(Guid scheduleId, DateTime startsAtUtc, DateTime endsAtUtc, Guid? ignoreAppointmentId = null, CancellationToken ct = default);
        Task<IReadOnlyList<Appointment>> GetCustomerHistoryAsync(Guid customerId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default);
    }
}