using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class CompanyPositionRepository : GenericRepository<CompanyPosition>, ICompanyPositionRepository
    {
        private readonly AppDbContext _ctx;
        public CompanyPositionRepository(AppDbContext ctx) : base(ctx) => _ctx = ctx;

        public Task<bool> ExistsByNameAsync(Guid companyId, string name, CancellationToken ct = default)
            => _ctx.Set<CompanyPosition>().AsNoTracking()
                .AnyAsync(p => p.CompanyId == companyId && p.Name == name, ct);

        public Task<CompanyPosition?> GetByNameAsync(Guid companyId, string name, CancellationToken ct = default)
            => _ctx.Set<CompanyPosition>().AsNoTracking()
                .FirstOrDefaultAsync(p => p.CompanyId == companyId && p.Name == name, ct);

        public async Task<string[]> ListActiveNamesAsync(Guid companyId, CancellationToken ct = default)
            => await _ctx.Set<CompanyPosition>().AsNoTracking()
                .Where(p => p.CompanyId == companyId && p.IsActive)
                .OrderBy(p => p.Name)
                .Select(p => p.Name)
                .ToArrayAsync(ct);

        public async Task<IReadOnlyList<CompanyPosition>> ListActiveAsync(Guid companyId, CancellationToken ct = default)
            => await _ctx.Set<CompanyPosition>().AsNoTracking()
                .Where(p => p.CompanyId == companyId && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync(ct);
    }
}