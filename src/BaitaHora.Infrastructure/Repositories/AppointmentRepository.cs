using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly AppDbContext _ctx;

        public AppointmentRepository(AppDbContext context) : base(context)
        {
            _ctx = context;
        }

        public async Task<IReadOnlyList<Appointment>> GetByScheduleAsync(Guid scheduleId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default)
        {
            var q = _ctx.Set<Appointment>().AsNoTracking().Where(a => a.ScheduleId == scheduleId);

            if (fromUtc.HasValue) q = q.Where(a => a.EndsAtUtc > fromUtc.Value);
            if (toUtc.HasValue) q = q.Where(a => a.StartsAtUtc < toUtc.Value);

            return await q.OrderBy(a => a.StartsAtUtc).ToListAsync(ct);
        }

        public async Task<bool> HasConflictAsync(Guid scheduleId, DateTime startsAtUtc, DateTime endsAtUtc, Guid? ignoreAppointmentId = null, CancellationToken ct = default)
        {
            var q = _ctx.Set<Appointment>().AsNoTracking()
                .Where(a => a.ScheduleId == scheduleId &&
                            a.EndsAtUtc > startsAtUtc &&
                            a.StartsAtUtc < endsAtUtc);

            if (ignoreAppointmentId.HasValue)
                q = q.Where(a => a.Id != ignoreAppointmentId.Value);

            return await q.AnyAsync(ct);
        }

        public async Task<IReadOnlyList<Appointment>> GetCustomerHistoryAsync(Guid customerUserId, DateTime? fromUtc = null, DateTime? toUtc = null, CancellationToken ct = default)
        {
            var q = _ctx.Set<Appointment>()
                .AsNoTracking()
                .Where(a => a.CustomerUserId == customerUserId);

            if (fromUtc.HasValue) q = q.Where(a => a.EndsAtUtc > fromUtc.Value);
            if (toUtc.HasValue) q = q.Where(a => a.StartsAtUtc < toUtc.Value);

            return await q.OrderByDescending(a => a.StartsAtUtc).ToListAsync(ct);
        }
    }
}