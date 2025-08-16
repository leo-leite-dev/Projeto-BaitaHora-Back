using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities.Scheduling;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class ServiceCatalogItemRepository : GenericRepository<ServiceCatalogItem>, IServiceCatalogItemRepository
    {
        private readonly AppDbContext _ctx;

        public ServiceCatalogItemRepository(AppDbContext context) : base(context)
        {
            _ctx = context;
        }

        public async Task<IReadOnlyList<ServiceCatalogItem>> GetByCompanyAsync(Guid companyId, bool onlyActive = true, CancellationToken ct = default)
        {
            var q = _ctx.Set<ServiceCatalogItem>().AsNoTracking().Where(s => s.CompanyId == companyId);
            if (onlyActive) q = q.Where(s => s.IsActive);
            return await q.OrderBy(s => s.Name).ToListAsync(ct);
        }

        public async Task<ServiceCatalogItem?> GetByNameAsync(Guid companyId, string name, CancellationToken ct = default)
        {
            var normalized = (name ?? string.Empty).Trim();
            return await _ctx.Set<ServiceCatalogItem>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.CompanyId == companyId && s.Name == normalized, ct);
        }

        public async Task<bool> ExistsNameAsync(Guid companyId, string name, CancellationToken ct = default)
        {
            var normalized = (name ?? string.Empty).Trim();
            return await _ctx.Set<ServiceCatalogItem>()
                .AsNoTracking()
                .AnyAsync(s => s.CompanyId == companyId && s.Name == normalized, ct);
        }
    }
}