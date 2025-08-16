using System.Security.Claims;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Domain.Entities.Companies;
using BaitaHora.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace BaitaHora.Api.Authorization
{
    public sealed class CompanyRoleHandler : AuthorizationHandler<CompanyRoleRequirement>
    {
        private readonly ICompanyPermissionService _perm;
        private readonly ICompanyContextResolver _resolver;

        public CompanyRoleHandler(ICompanyPermissionService perm, ICompanyContextResolver resolver)
        {
            _perm = perm ?? throw new ArgumentNullException(nameof(perm));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CompanyRoleRequirement requirement)
        {
            var httpCtx = (context.Resource as DefaultHttpContext) ??
                          (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext)?.HttpContext;

            if (httpCtx is null)
                return;

            var sub = httpCtx.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(sub, out var userId))
                return;

            var companyId = await _resolver.ResolveCompanyIdAsync(httpCtx);
            if (companyId is null || companyId == Guid.Empty)
                return;

            var effective = await _perm.GetEffectiveRoleAsync(companyId.Value, userId, httpCtx.RequestAborted);
            if (effective is null)
                return;

            if (CompanyMember.Rank(effective.Value) >= CompanyMember.Rank(requirement.MinRole))
            {
                context.Succeed(requirement);
            }
        }

    }
}
