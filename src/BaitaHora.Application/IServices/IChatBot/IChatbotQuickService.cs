using BaitaHora.Application.DTOs.Responses.Scheduling;

namespace BaitaHora.Application.IServices.IChatbot
{
    public interface IChatbotQuickService
    {
        Task<Guid> EnsureCustomerUserAsyncMinimal(Guid companyId, string fullName, string phoneE164, string? email, CancellationToken ct = default);

        Task<AppointmentResponse> BookForCustomerAsync(Guid companyId, Guid customerId, DateTime desiredWeekStartUtc, Guid? preferredProfessionalUserId = null, string? roleName = null, Guid? serviceId = null, CancellationToken ct = default);
    }
}