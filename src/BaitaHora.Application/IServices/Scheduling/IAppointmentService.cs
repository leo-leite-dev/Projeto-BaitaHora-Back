using BaitaHora.Application.DTOs.Requests.Scheduling;
using BaitaHora.Application.DTOs.Responses.Scheduling;

namespace BaitaHora.Application.IServices.Scheduling
{
    public interface IAppointmentService
    {
        Task<AppointmentResponse> CreatePendingSlotAsync(Guid scheduleId, CreatePendingSlotRequest request, CancellationToken ct = default);
        Task<AppointmentResponse> CreateAsync(CreateAppointmentRequest request, CancellationToken ct = default);
        Task AssignCustomerAsync(Guid appointmentId, Guid customerId, CancellationToken ct = default);
        Task UpdateStatusAsync(Guid appointmentId, string newStatus, CancellationToken ct = default);
        Task RescheduleAsync(Guid appointmentId, DateTime newStartsAtUtc, DateTime newEndsAtUtc, CancellationToken ct = default);
        Task CancelAsync(Guid appointmentId, string? reason = null, CancellationToken ct = default);
    }
}