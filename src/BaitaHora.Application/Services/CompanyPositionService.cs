using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.Services
{
    public sealed class CompanyPositionService : ICompanyPositionService
    {
        private readonly ICompanyRepository _companies;
        private readonly ICompanyPositionRepository _positions;
        private readonly ICompanyPermissionService _perm;
        private readonly IUnitOfWork _uow;

        public CompanyPositionService(
            ICompanyRepository companies,
            ICompanyPositionRepository positions,
            ICompanyPermissionService perm,
            IUnitOfWork uow)
        {
            _companies = companies;
            _positions = positions;
            _perm = perm;
            _uow = uow;
        }

        public async Task<Guid> CreateAsync(Guid companyId, Guid requesterUserId, string name, CompanyRole accessLevel, CancellationToken ct = default)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.", nameof(companyId));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome do cargo é obrigatório.", nameof(name));

            if (!await _perm.CanAsync(companyId, requesterUserId, CompanyRole.Owner, ct))
                throw new UnauthorizedAccessException("Apenas o dono pode criar cargos.");

            var company = await _companies.GetByIdAsync(companyId)
                          ?? throw new KeyNotFoundException("Empresa não encontrada.");

            var position = company.CreatePosition(name, accessLevel);

            await _positions.AddAsync(position, ct);
            await _uow.SaveChangesAsync(ct);

            return position.Id;
        }
    }
}