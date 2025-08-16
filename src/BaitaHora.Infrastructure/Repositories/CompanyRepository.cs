using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities.Companies;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(AppDbContext context) : base(context) { }

        public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        {
            var norm = name.Trim();
            return _context.Set<Company>()
                           .AsNoTracking()
                           .AnyAsync(c => EF.Functions.ILike(c.Name, norm), ct);
        }

        public Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct = default)
        {
            var norm = document.Trim();
            return _context.Set<Company>()
                           .AsNoTracking()
                           .AnyAsync(c => c.Document != null && c.Document == norm, ct);
        }

        public async Task AddImageAsync(CompanyImage image, CancellationToken ct = default)
        {
            await _context.Set<CompanyImage>().AddAsync(image, ct);
        }
    }
}