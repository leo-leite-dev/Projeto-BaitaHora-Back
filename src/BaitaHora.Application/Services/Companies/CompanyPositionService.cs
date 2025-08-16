using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.Services.Companies
{
    public sealed class CompanyPositionService : ICompanyPositionService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyPositionRepository _companyPosition;
        private readonly ICompanyPermissionService _companyPermission;
        private readonly IUnitOfWork _uow;

        public CompanyPositionService(
            ICompanyRepository companyRepository,
            ICompanyPositionRepository companyPosition,
            ICompanyPermissionService companyPermission,
            IUnitOfWork uow)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _companyPosition = companyPosition ?? throw new ArgumentNullException(nameof(companyRepository));
            _companyPermission = companyPermission ?? throw new ArgumentNullException(nameof(companyRepository));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<Guid> CreateAsync(Guid companyId, Guid requesterUserId, string name, CompanyRole accessLevel, CancellationToken ct = default)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.", nameof(companyId));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome do cargo é obrigatório.", nameof(name));

            if (!await _companyPermission.CanAsync(companyId, requesterUserId, CompanyRole.Owner, ct))
                throw new UnauthorizedAccessException("Apenas o dono pode criar cargos.");

            var company = await _companyRepository.GetByIdAsync(companyId)
                          ?? throw new KeyNotFoundException("Empresa não encontrada.");

            var position = company.CreatePosition(name, accessLevel);

            await _companyPosition.AddAsync(position, ct);
            await _uow.SaveChangesAsync(ct);

            return position.Id;
        }
    }
}