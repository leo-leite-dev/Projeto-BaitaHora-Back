using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class ScheduleRepository : GenericRepository<Schedule>, IScheduleRepository
    {
        private readonly AppDbContext _ctx;

        public ScheduleRepository(AppDbContext context) : base(context)
        {
            _ctx = context;
        }

        public async Task<Schedule?> GetByUserAsync(Guid userId, Guid companyId, CancellationToken ct = default)
        {
            return await _ctx.Set<Schedule>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId && s.CompanyId == companyId, ct);
        }

        public async Task<bool> ExistsForUserAsync(Guid userId, Guid companyId, CancellationToken ct = default)
        {
            return await _ctx.Set<Schedule>()
                .AsNoTracking()
                .AnyAsync(s => s.UserId == userId && s.CompanyId == companyId, ct);
        }

        public async Task<Schedule?> GetWithAppointmentsAsync(
            Guid userId, Guid companyId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            return await _ctx.Set<Schedule>()
                .Where(s => s.UserId == userId && s.CompanyId == companyId)
                .Include(s => s.Appointments
                    .Where(a => a.StartsAtUtc < toUtc && a.EndsAtUtc > fromUtc))
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);
        }
    }
}