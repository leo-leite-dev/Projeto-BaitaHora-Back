using BaitaHora.Application.DTOs.Requests.Scheduling;
using BaitaHora.Application.DTOs.Responses.Scheduling;

namespace BaitaHora.Application.IServices.Scheduling
{
    public interface IServiceCatalogService
    {
        Task<ServiceCatalogItemResponse> CreateAsync(Guid companyId, CreateServiceCatalogItemRequest request, CancellationToken ct = default);
        Task<IReadOnlyList<ServiceCatalogItemResponse>> GetByCompanyAsync(Guid companyId, bool onlyActive = true, CancellationToken ct = default);
        Task DeactivateAsync(Guid serviceId, CancellationToken ct = default);
    }
}