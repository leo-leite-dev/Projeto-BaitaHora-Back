using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.DTOs.Requests.Company
{
    public sealed class CompanyPositionRequest
    {
        public string Name { get; init; } = string.Empty;
        public CompanyRole AccessLevel { get; init; } = CompanyRole.Staff;
    }
}