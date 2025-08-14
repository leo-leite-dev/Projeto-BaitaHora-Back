using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public class CompanyMemberRepository : ICompanyMemberRepository
    {
        private readonly AppDbContext _context;

        public CompanyMemberRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<CompanyMember?> GetAsync(Guid companyId, Guid userId)
        {
            return _context.CompanyMembers
                      .AsNoTracking()
                      .FirstOrDefaultAsync(m => m.CompanyId == companyId && m.UserId == userId && m.IsActive);
        }

        public async Task AddAsync(CompanyMember member)
        {
            await _context.CompanyMembers.AddAsync(member);
            await _context.SaveChangesAsync();
        }

        public Task<bool> IsActiveAsync(Guid companyId, Guid userId)
        {
            return _context.CompanyMembers.AnyAsync(m => m.CompanyId == companyId && m.UserId == userId && m.IsActive);
        }
    }
}