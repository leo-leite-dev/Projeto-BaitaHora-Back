namespace BaitaHora.Application.Services
{
    using BaitaHora.Application.DTOs.Requests.Company;
    using BaitaHora.Application.IRepositories;
    using BaitaHora.Application.IServices;
    using BaitaHora.Domain.Entities;
    using BaitaHora.Domain.Enums;
    using BaitaHora.Domain.Factories;

    public sealed class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companies;
        private readonly ICompanyPositionRepository _positions;
        private readonly ICompanyMemberRepository _members;
        private readonly ICompanyPermissionService _perm;
        private readonly IUnitOfWork _uow;

        public CompanyService(
            ICompanyRepository companyRepository,
            ICompanyPositionRepository positionRepository,
            ICompanyMemberRepository memberRepository,
            ICompanyPermissionService permissionService,
            IUnitOfWork unitOfWork)
        {
            _companies = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _positions = positionRepository ?? throw new ArgumentNullException(nameof(positionRepository));
            _members = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
            _perm = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _uow = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Company> CreateCompanyAsync(CompanyRequest request, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("Nome da empresa é obrigatório.", nameof(request.Name));

            var a = request.Address ?? throw new ArgumentException("Endereço é obrigatório.", nameof(request.Address));
            var address = AddressFactory.Create(
                a.Street, a.Number, a.District, a.City, a.State, a.ZipCode, a.Complement);

            var company = Company.Create(request.Name.Trim(), address, request.Document);

            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
                company.SetImage(new CompanyImage(company.Id, request.ImageUrl.Trim()));

            await _companies.AddAsync(company, ct);
            await _uow.SaveChangesAsync(ct);

            return company;
        }

        public async Task AddMemberAsync(Guid companyId, Guid userId, CompanyRole role, bool isActive = true, CancellationToken ct = default)
        {
            var company = await _companies.GetByIdAsync(companyId)
                ?? throw new KeyNotFoundException("Empresa não encontrada.");

            var existing = await _members.GetAsync(companyId, userId, ct);
            if (existing is not null)
                throw new InvalidOperationException("Usuário já é membro desta empresa.");

            var member = CompanyMember.Create(companyId, userId, role);

            if (isActive) member.Activate(); else member.Deactivate();

            await _members.AddAsync(member, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task UpdateCompanyAsync(Guid companyId, Guid requesterUserId, CompanyRequest companyRequest, CancellationToken ct = default)
        {
            if (!await _perm.CanAsync(companyId, requesterUserId, CompanyRole.Owner, ct))
                throw new UnauthorizedAccessException("Apenas o dono pode editar os dados da empresa.");

            var company = await _companies.GetByIdAsync(companyId)
                ?? throw new KeyNotFoundException("Empresa não encontrada.");

            if (!string.IsNullOrWhiteSpace(companyRequest.Name))
                company.UpdateName(companyRequest.Name);

            if (companyRequest.Document is not null)
                company.UpdateDocument(companyRequest.Document);

            if (companyRequest.Address is not null)
            {
                var a = companyRequest.Address;
                var newAddress = AddressFactory.Create(
                    a.Street, a.Number, a.District, a.City, a.State, a.ZipCode, a.Complement);
                company.UpdateAddress(newAddress);
            }

            await _companies.UpdateAsync(company);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task SetMemberPrimaryPositionAsync(Guid companyId, Guid requesterUserId, Guid memberUserId, Guid positionId, CancellationToken ct = default)
        {
            if (!await _perm.CanAsync(companyId, requesterUserId, CompanyRole.Manager, ct))
                throw new UnauthorizedAccessException("Permissão insuficiente para atribuir cargo.");

            var member = await _members.GetWithPositionAsync(companyId, memberUserId, ct)
                ?? throw new KeyNotFoundException("Membro não encontrado.");

            if (!member.IsActive)
                throw new InvalidOperationException("Membro inativo.");

            var position = await _positions.GetByIdAsync(positionId)
                ?? throw new KeyNotFoundException("Cargo não encontrado.");

            if (!position.IsActive)
                throw new InvalidOperationException("Cargo inativo.");

            member.SetPrimaryPosition(position);

            await _members.UpdateAsync(member);
            await _uow.SaveChangesAsync(ct);
        }
    }
}