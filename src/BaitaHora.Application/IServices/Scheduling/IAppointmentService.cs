using BaitaHora.Application.DTOs.Commands.Scheduling;
using BaitaHora.Application.DTOs.Responses.Scheduling;

namespace BaitaHora.Application.IServices.Scheduling
{
    public interface IAppointmentService
    {
        Task<AppointmentResponse> CreatePendingSlotAsync(CreatePendingSlotCommand cmd, CancellationToken ct = default);
        Task<AppointmentResponse> CreateAsync(CreateAppointmentCommand cmd, CancellationToken ct = default);
        Task AssignCustomerAsync(Guid appointmentId, Guid customerId, CancellationToken ct = default);
        Task ConfirmAsync(Guid appointmentId, CancellationToken ct = default);
        Task CompleteAsync(Guid appointmentId, CancellationToken ct = default);
        Task CancelAsync(Guid appointmentId, string? reason = null, CancellationToken ct = default);
        Task RescheduleAsync(Guid appointmentId, DateTime newStartsAtUtc, DateTime newEndsAtUtc, CancellationToken ct = default);
    }
}