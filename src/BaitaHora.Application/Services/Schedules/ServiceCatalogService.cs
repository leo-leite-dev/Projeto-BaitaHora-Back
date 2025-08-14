using AutoMapper;
using BaitaHora.Application.DTOs.Requests.Scheduling;
using BaitaHora.Application.DTOs.Responses.Scheduling;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.Services.Scheduling
{
    public sealed class ServiceCatalogService : IServiceCatalogService
    {
        private readonly IServiceCatalogItemRepository _repo;
        private readonly IMapper _mapper;

        public ServiceCatalogService(IServiceCatalogItemRepository repo, IMapper mapper)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ServiceCatalogItemResponse> CreateAsync(Guid companyId, CreateServiceCatalogItemRequest request, CancellationToken ct = default)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");

            if (await _repo.ExistsNameAsync(companyId, request.Name, ct))
                throw new InvalidOperationException("Já existe um serviço com esse nome.");

            var entity = new ServiceCatalogItem(companyId, request.Name, request.DurationMinutes, request.Price);
            await _repo.AddAsync(entity);

            return _mapper.Map<ServiceCatalogItemResponse>(entity);
        }

        public async Task<IReadOnlyList<ServiceCatalogItemResponse>> GetByCompanyAsync(Guid companyId, bool onlyActive = true, CancellationToken ct = default)
        {
            var list = await _repo.GetByCompanyAsync(companyId, onlyActive, ct);
            return list.Select(_mapper.Map<ServiceCatalogItemResponse>).ToList();
        }

        public async Task DeactivateAsync(Guid serviceId, CancellationToken ct = default)
        {
            var item = await _repo.GetByIdAsync(serviceId) ?? throw new KeyNotFoundException("Serviço não encontrado.");
            item.Deactivate();
            await _repo.UpdateAsync(item);
        }
    }
}