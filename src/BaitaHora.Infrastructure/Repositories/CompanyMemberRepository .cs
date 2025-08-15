using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class CompanyMemberRepository : GenericRepository<CompanyMember>, ICompanyMemberRepository
    {
        private readonly AppDbContext _ctx;
        public CompanyMemberRepository(AppDbContext ctx) : base(ctx) { _ctx = ctx; }

        public Task<CompanyMember?> GetAsync(Guid companyId, Guid userId, CancellationToken ct = default)
            => _ctx.Set<CompanyMember>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.UserId == userId, ct);

        public Task<CompanyMember?> GetWithPositionAsync(Guid companyId, Guid userId, CancellationToken ct = default)
            => _ctx.Set<CompanyMember>().AsNoTracking()
                .Include(m => m.PrimaryPosition)
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.UserId == userId, ct);

        public Task<bool> IsActiveAsync(Guid companyId, Guid userId, CancellationToken ct = default)
            => _ctx.Set<CompanyMember>().AsNoTracking()
                .AnyAsync(x => x.CompanyId == companyId && x.UserId == userId && x.IsActive, ct);

        public Task<CompanyMember?> FindAnyActiveAsync(Guid companyId, CancellationToken ct = default)
            => _ctx.Set<CompanyMember>().AsNoTracking()
                .Include(m => m.PrimaryPosition)
                .Where(m => m.CompanyId == companyId
                            && m.IsActive
                            && (m.PrimaryPositionId == null || m.PrimaryPosition!.IsActive))
                .OrderBy(m => m.JoinedAt)
                .FirstOrDefaultAsync(ct);

        public Task<CompanyMember?> FindAnyActiveByPositionNameAsync(Guid companyId, string positionName, CancellationToken ct = default)
        {
            var norm = (positionName ?? string.Empty).Trim().ToLower();
            return _ctx.Set<CompanyMember>().AsNoTracking()
                .Include(m => m.PrimaryPosition)
                .Where(m => m.CompanyId == companyId
                            && m.IsActive
                            && m.PrimaryPositionId != null
                            && m.PrimaryPosition!.IsActive
                            && m.PrimaryPosition!.Name.ToLower() == norm)
                .OrderBy(m => m.JoinedAt)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<List<string>> ListActiveRolesAsync(Guid companyId, CancellationToken ct = default)
        {
            return await _ctx.Set<CompanyMember>()
                .AsNoTracking()
                .Where(m => m.CompanyId == companyId && m.IsActive)
                .Select(m => m.PrimaryPosition != null && m.PrimaryPosition.IsActive
                    ? m.PrimaryPosition.Name
                    : null)
                .Where(name => name != null)
                .Cast<string>()
                .ToListAsync(ct);
        }

    }
}