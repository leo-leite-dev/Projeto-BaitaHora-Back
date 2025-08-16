using BaitaHora.Domain.Entities;
using BaitaHora.Domain.Entities.Companies.Customers;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext _ctx;
        public CustomerRepository(AppDbContext ctx) : base(ctx) => _ctx = ctx;

        public Task<Customer?> GetByPhoneAsync(string phoneE164, CancellationToken ct = default)
            => _ctx.Set<Customer>().AsNoTracking()
                  .FirstOrDefaultAsync(c => c.PhoneE164 == phoneE164, ct);

        public Task<bool> ExistsByPhoneAsync(string phoneE164, CancellationToken ct = default)
            => _ctx.Set<Customer>().AsNoTracking().AnyAsync(c => c.PhoneE164 == phoneE164, ct);

        public async Task<Guid> EnsureCustomerMinimalAsync(string phoneE164, string? name = null, string? email = null, CancellationToken ct = default)
        {
            var existing = await _ctx.Set<Customer>().FirstOrDefaultAsync(c => c.PhoneE164 == phoneE164, ct);
            if (existing != null) return existing.Id;

            var customer = new Customer(phoneE164, name);
            await _ctx.Set<Customer>().AddAsync(customer, ct);
            await _ctx.SaveChangesAsync(ct);
            return customer.Id;
        }
    }
}