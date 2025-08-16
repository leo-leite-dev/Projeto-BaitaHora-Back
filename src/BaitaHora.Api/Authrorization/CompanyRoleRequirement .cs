using BaitaHora.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BaitaHora.Api.Authorization
{
    public sealed class CompanyRoleRequirement : IAuthorizationRequirement
    {
        public CompanyRole MinRole { get; }
        public CompanyRoleRequirement(CompanyRole minRole) => MinRole = minRole;
    }
}