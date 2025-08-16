using BaitaHora.Application.DTOs.Requests;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.DTOs.Auth.Requests
{
    public sealed class RegisterEmployeeRequest
    {
        public Guid CompanyId { get; init; }
        public CompanyRole Role { get; init; } = CompanyRole.Viewer;
        public UserRequest User { get; init; } = new();
    }
}