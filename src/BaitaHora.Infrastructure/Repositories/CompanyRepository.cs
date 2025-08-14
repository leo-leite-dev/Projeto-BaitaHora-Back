using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public class CompanyRepository : GenericRepository<Company>, ICompanyRepository
    {
        private readonly AppDbContext _context;

        public CompanyRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            var norm = name.Trim().ToLower();
            return _context.Companies.AnyAsync(c => c.Name.ToLower() == norm);
        }

        public Task<bool> ExistsByDocumentAsync(string document)
        {
            var norm = document.Trim();
            return _context.Companies.AnyAsync(c => c.Document != null && c.Document == norm);
        }

        public async Task AddImageAsync(CompanyImage image)
        {
            await _context.CompanyImages.AddAsync(image);
            await _context.SaveChangesAsync();
        }
    }
}