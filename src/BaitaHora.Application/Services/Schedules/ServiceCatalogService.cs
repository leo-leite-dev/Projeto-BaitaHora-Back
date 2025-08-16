using AutoMapper;
using BaitaHora.Application.DTOs.Requests.Scheduling;
using BaitaHora.Application.DTOs.Responses.Scheduling;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Domain.Entities.Scheduling;

namespace BaitaHora.Application.Services.Scheduling
{
    public sealed class ServiceCatalogService : IServiceCatalogService
    {
        private readonly IServiceCatalogItemRepository _serviceCatalogItemRepository;
        private readonly IMapper _mapper;

        public ServiceCatalogService(
            IServiceCatalogItemRepository serviceCatalogItemRepository,
            IMapper mapper)
        {
            _serviceCatalogItemRepository = serviceCatalogItemRepository ?? throw new ArgumentNullException(nameof(serviceCatalogItemRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ServiceCatalogItemResponse> CreateAsync(Guid companyId, CreateServiceCatalogItemRequest request, CancellationToken ct = default)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.", nameof(companyId));

            if (await _serviceCatalogItemRepository.ExistsNameAsync(companyId, request.Name, ct))
                throw new InvalidOperationException("Já existe um serviço com esse nome.");

            var entity = new ServiceCatalogItem(companyId, request.Name, request.DurationMinutes, request.Price);
            await _serviceCatalogItemRepository.AddAsync(entity, ct);

            return _mapper.Map<ServiceCatalogItemResponse>(entity);
        }

        public async Task<IReadOnlyList<ServiceCatalogItemResponse>> GetByCompanyAsync(Guid companyId, bool onlyActive = true, CancellationToken ct = default)
        {
            var list = await _serviceCatalogItemRepository.GetByCompanyAsync(companyId, onlyActive, ct);
            return list.Select(x => _mapper.Map<ServiceCatalogItemResponse>(x)).ToList();
        }

        public async Task DeactivateAsync(Guid serviceId, CancellationToken ct = default)
        {
            var item = await _serviceCatalogItemRepository.GetByIdAsync(serviceId, ct)
                ?? throw new KeyNotFoundException("Serviço não encontrado.");

            item.Deactivate();
            await _serviceCatalogItemRepository.UpdateAsync(item, ct);
        }

        public async Task RenameAsync(Guid serviceId, string newName, CancellationToken ct = default)
        {
            var item = await _serviceCatalogItemRepository.GetByIdAsync(serviceId, ct)
                ?? throw new KeyNotFoundException("Serviço não encontrado.");

            if (await _serviceCatalogItemRepository.ExistsNameAsync(item.CompanyId, newName, ct))
                throw new InvalidOperationException("Já existe um serviço com esse nome.");

            item.Rename(newName);
            await _serviceCatalogItemRepository.UpdateAsync(item, ct);
        }

        public async Task ChangeDurationAsync(Guid serviceId, int durationMinutes, CancellationToken ct = default)
        {
            var item = await _serviceCatalogItemRepository.GetByIdAsync(serviceId, ct)
                ?? throw new KeyNotFoundException("Serviço não encontrado.");

            item.ChangeDuration(durationMinutes);
            await _serviceCatalogItemRepository.UpdateAsync(item, ct);
        }

        public async Task ChangePriceAsync(Guid serviceId, decimal? price, CancellationToken ct = default)
        {
            var item = await _serviceCatalogItemRepository.GetByIdAsync(serviceId, ct)
                ?? throw new KeyNotFoundException("Serviço não encontrado.");

            item.ChangePrice(price);
            await _serviceCatalogItemRepository.UpdateAsync(item, ct);
        }
    }
}