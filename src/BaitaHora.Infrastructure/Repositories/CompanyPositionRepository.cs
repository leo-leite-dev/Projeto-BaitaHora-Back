using BaitaHora.Domain.Entities.Companies;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class CompanyPositionRepository : GenericRepository<CompanyPosition>, ICompanyPositionRepository
    {
        public CompanyPositionRepository(AppDbContext ctx) : base(ctx) { }

        public Task<bool> ExistsByNameAsync(Guid companyId, string name, CancellationToken ct = default)
        {
            var norm = (name ?? string.Empty).Trim();
            return _set.AsNoTracking()
                       .AnyAsync(p => p.CompanyId == companyId
                                   && EF.Functions.ILike(p.Name, norm), ct);
        }

        public Task<CompanyPosition?> GetByNameAsync(Guid companyId, string name, CancellationToken ct = default)
        {
            var norm = (name ?? string.Empty).Trim();
            return _set.AsNoTracking()
                       .FirstOrDefaultAsync(p => p.CompanyId == companyId
                                              && EF.Functions.ILike(p.Name, norm), ct);
        }

        public Task<string[]> ListActiveNamesAsync(Guid companyId, CancellationToken ct = default)
            => _set.AsNoTracking()
                   .Where(p => p.CompanyId == companyId && p.IsActive)
                   .OrderBy(p => p.Name)
                   .Select(p => p.Name)
                   .ToArrayAsync(ct);

        public Task<IReadOnlyList<CompanyPosition>> ListActiveAsync(Guid companyId, CancellationToken ct = default)
            => _set.AsNoTracking()
                   .Where(p => p.CompanyId == companyId && p.IsActive)
                   .OrderBy(p => p.Name)
                   .ToListAsync(ct)
                   .ContinueWith(t => (IReadOnlyList<CompanyPosition>)t.Result, ct);

        public Task<CompanyPosition?> GetByIdWithinCompanyAsync(Guid companyId, Guid positionId, CancellationToken ct = default)
            => _set.AsNoTracking()
                   .FirstOrDefaultAsync(p => p.Id == positionId && p.CompanyId == companyId, ct);
    }
}