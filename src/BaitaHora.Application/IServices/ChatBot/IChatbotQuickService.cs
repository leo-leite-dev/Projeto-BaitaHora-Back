using BaitaHora.Application.DTOs.Responses.Scheduling;

namespace BaitaHora.Application.IServices.IChatbot
{
    public interface IChatbotQuickService
    {
        Task<Guid> EnsureCustomerUserAsyncMinimal(Guid companyId, string fullName, string phoneE164, string? email, CancellationToken ct = default);
        Task<AppointmentResponse> BookPendingSlotAsync(Guid companyId, DateTime desiredWeekStartUtc, Guid? preferredProfessionalUserId = null, string? roleName = null, Guid? serviceId = null, int? overrideDurationMinutes = null, CancellationToken ct = default);
        Task<AppointmentResponse> BookConfirmedAsync(Guid companyId, Guid customerId, DateTime desiredWeekStartUtc, Guid? preferredProfessionalUserId = null, string? roleName = null, Guid? serviceId = null, int? overrideDurationMinutes = null, CancellationToken ct = default);
        Task<AppointmentResponse> AssignCustomerToAppointmentAsync(Guid companyId, Guid appointmentId, Guid customerId, Guid? serviceId = null, CancellationToken ct = default);
        Task<AppointmentResponse> BookForCustomerLegacyAsync(Guid companyId, Guid customerUserId, DateTime desiredWeekStartUtc, Guid? preferredProfessionalUserId = null, string? roleName = null, Guid? serviceId = null, CancellationToken ct = default);
    }
}