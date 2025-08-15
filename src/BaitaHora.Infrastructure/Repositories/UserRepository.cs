using System.Text.RegularExpressions;
using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _ctx;

        public UserRepository(AppDbContext context) : base(context)
        {
            _ctx = context;
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            var norm = (email ?? string.Empty).Trim().ToLowerInvariant();
            return _ctx.Set<User>().AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == norm, ct);
        }

        public Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            var norm = (username ?? string.Empty).Trim().ToLowerInvariant();
            return _ctx.Set<User>().AsNoTracking()
                .FirstOrDefaultAsync(u => (u.Username ?? string.Empty).ToLower() == norm, ct);
        }

        public Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default)
        {
            var norm = (username ?? string.Empty).Trim().ToLowerInvariant();
            return _ctx.Set<User>().AsNoTracking()
                .AnyAsync(u => (u.Username ?? string.Empty).ToLower() == norm, ct);
        }

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        {
            var norm = (email ?? string.Empty).Trim().ToLowerInvariant();
            return _ctx.Set<User>().AsNoTracking()
                .AnyAsync(u => u.Email.ToLower() == norm, ct);
        }

        public Task<User?> GetByPhoneAsync(string phoneE164, CancellationToken ct = default)
        {
            var norm = (phoneE164 ?? string.Empty).Trim();
            return _ctx.Set<User>().AsNoTracking()
                .Join(_ctx.Set<UserProfile>(),
                      u => u.ProfileId,
                      up => up.Id,
                      (u, up) => new { u, up })
                .Where(x => x.up.Phone == norm)
                .Select(x => x.u)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<string> GenerateUsernameAsync(string fullName, CancellationToken ct = default)
        {
            var baseSlug = Slugify(fullName);
            if (string.IsNullOrWhiteSpace(baseSlug))
                baseSlug = "user";

            var candidate = baseSlug;
            var i = 0;
            while (await ExistsByUsernameAsync(candidate, ct))
            {
                i++;
                candidate = $"{baseSlug}{i}";
            }
            return candidate;

            static string Slugify(string s)
            {
                s = (s ?? string.Empty).Trim().ToLowerInvariant();
                s = Regex.Replace(s, @"\s+", ".");
                s = Regex.Replace(s, @"[^a-z0-9\.]+", "");
                s = Regex.Replace(s, @"\.{2,}", ".");
                return s.Trim('.');
            }
        }
    }
}