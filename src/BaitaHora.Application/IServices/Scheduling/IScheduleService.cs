using BaitaHora.Application.DTOs.Responses.Scheduling;

namespace BaitaHora.Application.IServices.Scheduling
{
    public interface IScheduleService
    {
        Task<ScheduleResponse> EnsureScheduleAsync(Guid userId, Guid companyId, CancellationToken ct = default);
        Task<ScheduleResponse?> GetByUserAsync(Guid userId, Guid companyId, CancellationToken ct = default);
        Task<IReadOnlyList<AppointmentResponse>> GetAppointmentsAsync(Guid userId, Guid companyId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);
        Task DeactivateAsync(Guid scheduleId, CancellationToken ct = default);
    }
}