using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<Customer?> GetByPhoneAsync(string phoneE164, CancellationToken ct = default);
    Task<bool> ExistsByPhoneAsync(string phoneE164, CancellationToken ct = default);

    Task<Guid> EnsureCustomerMinimalAsync(string phoneE164, string? name = null, string? email = null, CancellationToken ct = default);
}