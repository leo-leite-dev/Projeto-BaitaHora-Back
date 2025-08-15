using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class CompanyCustomerProfessionalRepository : GenericRepository<CompanyCustomerProfessional>, ICompanyCustomerProfessionalRepository
    {
        private readonly AppDbContext _ctx;
        public CompanyCustomerProfessionalRepository(AppDbContext ctx) : base(ctx) { _ctx = ctx; }

        public Task<CompanyCustomerProfessional?> GetPrimaryAsync(Guid companyId, Guid customerId, CancellationToken ct = default)
            => _ctx.Set<CompanyCustomerProfessional>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.CustomerId == customerId && x.IsPrimary && x.IsActive, ct);

        public async Task<IReadOnlyList<CompanyCustomerProfessional>> ListByCustomerAsync(Guid companyId, Guid customerId, bool onlyActive = true, CancellationToken ct = default)
        {
            var q = _ctx.Set<CompanyCustomerProfessional>().AsNoTracking()
                .Where(x => x.CompanyId == companyId && x.CustomerId == customerId);
            if (onlyActive) q = q.Where(x => x.IsActive);
            return await q.ToListAsync(ct);
        }

        public Task<bool> ExistsAsync(Guid companyId, Guid customerId, Guid professionalUserId, CancellationToken ct = default)
            => _ctx.Set<CompanyCustomerProfessional>().AsNoTracking()
                .AnyAsync(x => x.CompanyId == companyId && x.CustomerId == customerId && x.ProfessionalUserId == professionalUserId, ct);
    }
}