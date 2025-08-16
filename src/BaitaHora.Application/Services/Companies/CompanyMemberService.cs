using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Domain.Entities.Companies;
using BaitaHora.Domain.Enums;

public sealed class CompanyMemberService : ICompanyMemberService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICompanyMemberRepository _companyMemberRepository;
    private readonly ICompanyPositionRepository _companyPositionRepository;
    private readonly ICompanyPermissionService _companyPermissionService;
    private readonly IUnitOfWork _uow;

    public CompanyMemberService(
        ICompanyRepository companyRepository,
        ICompanyMemberRepository companyMemberRepository,
        ICompanyPositionRepository companyPositionRepository,
        ICompanyPermissionService companyPermissionService,
        IUnitOfWork uow)
    {
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        _companyMemberRepository = companyMemberRepository ?? throw new ArgumentNullException(nameof(companyMemberRepository));
        _companyPositionRepository = companyPositionRepository ?? throw new ArgumentNullException(nameof(companyPositionRepository));
        _companyPermissionService = companyPermissionService ?? throw new ArgumentNullException(nameof(companyPermissionService));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    }

    public async Task AddMemberAsync(Guid companyId, Guid requesterUserId, Guid userId, CompanyRole role, bool isActive = true, CancellationToken ct = default)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.", nameof(companyId));
        if (userId == Guid.Empty) throw new ArgumentException("UserId inválido.", nameof(userId));
        if (!Enum.IsDefined(typeof(CompanyRole), role)) throw new ArgumentException("Role inválido.", nameof(role));

        var existsCompany = await _companyRepository.ExistsAsync(companyId, ct);
        if (!existsCompany)
            throw new KeyNotFoundException("Empresa não encontrada.");

        if (role != CompanyRole.Owner)
        {
            var required = CompanyRole.Manager;
            var allowed = await _companyPermissionService.CanAsync(companyId, requesterUserId, required, ct);
            if (!allowed)
                throw new UnauthorizedAccessException("Permissão insuficiente para adicionar membro.");
        }

        var existing = await _companyMemberRepository.GetAsync(companyId, userId, ct);
        if (existing is not null)
            throw new InvalidOperationException("Usuário já é membro desta empresa.");

        if (role == CompanyRole.Owner)
        {
            var alreadyHasActiveOwner = await _companyMemberRepository.AnyByRoleAsync(
                companyId,
                CompanyRole.Owner,
                onlyActive: true,
                ct: ct);

            if (alreadyHasActiveOwner)
                throw new InvalidOperationException("A empresa já possui um proprietário ativo.");
        }

        var member = CompanyMember.Create(companyId, userId, role);
        if (isActive) member.Activate(); else member.Deactivate();

        await _companyMemberRepository.AddAsync(member, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task SetMemberPositionAsync(Guid companyId, Guid requesterUserId, Guid memberUserId, Guid positionId, CancellationToken ct = default)
    {
        if (!await _companyPermissionService.CanAsync(companyId, requesterUserId, CompanyRole.Manager, ct))
            throw new UnauthorizedAccessException("Permissão insuficiente para atribuir cargo.");

        var member = await _companyMemberRepository.GetWithPositionAsync(companyId, memberUserId, ct)
            ?? throw new KeyNotFoundException("Membro não encontrado.");

        if (!member.IsActive)
            throw new InvalidOperationException("Membro inativo.");

        var position = await _companyPositionRepository.GetByIdAsync(positionId, ct)
            ?? throw new KeyNotFoundException("Cargo não encontrado.");

        if (!position.IsActive)
            throw new InvalidOperationException("Cargo inativo.");

        if (position.CompanyId != companyId)
            throw new InvalidOperationException("Cargo pertence a outra empresa.");

        if (member.PrimaryPositionId == position.Id)
            return;

        member.SetPrimaryPosition(position);

        await _companyMemberRepository.UpdateAsync(member, ct);
        await _uow.SaveChangesAsync(ct);
    }
}