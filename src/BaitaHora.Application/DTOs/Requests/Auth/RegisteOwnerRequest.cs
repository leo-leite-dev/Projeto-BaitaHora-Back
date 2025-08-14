using BaitaHora.Application.DTOs.Requests.Company;

namespace BaitaHora.Application.DTOs.Requests.Auth
{
    public sealed class RegisteOwnerRequest
    {
        public UserRequest User { get; init; } = new();
        public CompanyRequest Company { get; init; } = new();
    }
}