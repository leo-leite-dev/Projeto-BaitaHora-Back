// Application/DTOs/Chatbot/Requests/CreateCustomerQuickRequest.cs
namespace BaitaHora.Application.DTOs.Chatbot.Requests
{
    public sealed record CreateCustomerRequest
    {
        public Guid CompanyId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string PhoneE164 { get; init; } = string.Empty;

        public Guid? ProfessionalUserId { get; init; }
        public string? RoleName { get; init; }
    }
}
