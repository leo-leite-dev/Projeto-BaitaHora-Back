using BaitaHora.Application.DTOs.Commands.Companies;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Domain.Entities.Commons.ValueObjects;
using BaitaHora.Domain.Entities.Companies;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.Services.Companies
{
    public sealed class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly ICompanyPermissionService _permissonService;
        private readonly IUnitOfWork _uow;

        public CompanyService(
            ICompanyRepository companyRepository,
            ICompanyPermissionService permissionService,
            IUnitOfWork unitOfWork)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _permissonService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _uow = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Company> CreateCompanyAsync(CreateCompanyCommand cmd, CancellationToken ct = default)
        {
            var name = cmd.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome da empresa é obrigatório.", nameof(cmd.Name));

            var a = cmd.Address;

            var address = Address.Create(
                a.Street, a.Number, a.District, a.City, a.State, a.ZipCode, a.Complement
            );

            var document = cmd.Document?.Trim();
            var imageUrl = string.IsNullOrWhiteSpace(cmd.ImageUrl) ? null : cmd.ImageUrl.Trim();

            var company = Company.Create(name, address, document);

            if (!string.IsNullOrWhiteSpace(imageUrl))
                company.SetImage(new CompanyImage(company.Id, imageUrl));

            await _companyRepository.AddAsync(company, ct);
            await _uow.SaveChangesAsync(ct);
            return company;
        }

        public async Task UpdateCompanyAsync(UpdateCompanyCommand cmd, CancellationToken ct = default)
        {
            if (!await _permissonService.CanAsync(cmd.CompanyId, cmd.RequesterUserId, CompanyRole.Owner, ct))
                throw new UnauthorizedAccessException("Apenas o dono pode editar os dados da empresa.");

            var company = await _companyRepository.GetByIdAsync(cmd.CompanyId)
                ?? throw new KeyNotFoundException("Empresa não encontrada.");

            var name = cmd.Name?.Trim();
            var document = cmd.Document?.Trim();
            var imageUrl = string.IsNullOrWhiteSpace(cmd.ImageUrl) ? null : cmd.ImageUrl.Trim();

            if (!string.IsNullOrWhiteSpace(name))
                company.UpdateName(name);

            if (cmd.Document is not null)  
                company.UpdateDocument(document ?? string.Empty);

            if (cmd.Address is not null)
            {
                var a = cmd.Address;
                var newAddress = Address.Create(a.Street, a.Number, a.District, a.City, a.State, a.ZipCode, a.Complement);
                company.UpdateAddress(newAddress);
            }

            if (cmd.ImageUrl is not null)
            {
                if (!string.IsNullOrWhiteSpace(imageUrl))
                    company.SetImage(new CompanyImage(company.Id, imageUrl));
                // else
                //     company.RemoveImage();  
            }

            await _companyRepository.UpdateAsync(company);
            await _uow.SaveChangesAsync(ct);
        }
    }
}