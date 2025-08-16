using BaitaHora.Application.DTOs.Requests;
using BaitaHora.Application.DTOs.Requests.Company;

namespace BaitaHora.Application.DTOs.Auth.Requests
{
    public sealed class RegisterOwnerRequest
    {
        public UserRequest User { get; init; } = new();
        public CompanyRequest Company { get; init; } = new();
    }
}