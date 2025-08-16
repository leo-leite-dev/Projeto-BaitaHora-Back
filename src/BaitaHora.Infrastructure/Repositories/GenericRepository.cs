using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities.Commons;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T> : IGenericRepository<T> where T : Base
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _set;

    public GenericRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _set = _context.Set<T>();
    }

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _set.FindAsync(new object?[] { id }, ct).AsTask();

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        entity.MarkCreated();
        await _set.AddAsync(entity, ct);
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        entity.Touch();
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public Task<List<T>> GetAllAsync(CancellationToken ct = default)
        => _set.AsNoTracking().ToListAsync(ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => _set.AnyAsync(e => e.Id == id, ct);
}