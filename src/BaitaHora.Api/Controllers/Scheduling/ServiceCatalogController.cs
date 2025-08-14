using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Helpers;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Application.DTOs.Requests.Scheduling;

namespace BaitaHora.Api.Controllers.Scheduling
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class ServiceCatalogController : ControllerBase
    {
        private readonly IServiceCatalogService _service;
        private readonly ILogger<ServiceCatalogController> _logger;

        public ServiceCatalogController(IServiceCatalogService service, ILogger<ServiceCatalogController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("companies/{companyId:guid}")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> Create([FromRoute] Guid companyId, [FromBody] CreateServiceCatalogItemRequest request, CancellationToken ct)
        {
            try
            {
                var created = await _service.CreateAsync(companyId, request, ct);
                return Ok(ApiResponseHelper.CreateSuccess(created, "Serviço criado"));
            }
            catch (InvalidOperationException ioe)
            {
                return ApiResponseHelper.CreateError("Conflito", ioe.Message, 409);
            }
            catch (ArgumentException ae)
            {
                return ApiResponseHelper.CreateError("Requisição inválida", ae.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar serviço para company {CompanyId}", companyId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpGet("companies/{companyId:guid}")]
        public async Task<IActionResult> GetByCompany([FromRoute] Guid companyId, [FromQuery] bool onlyActive = true, CancellationToken ct = default)
        {
            try
            {
                var items = await _service.GetByCompanyAsync(companyId, onlyActive, ct);
                return Ok(ApiResponseHelper.CreateSuccess(items, "Serviços carregados"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar serviços para company {CompanyId}", companyId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpDelete("{serviceId:guid}")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> Deactivate([FromRoute] Guid serviceId, CancellationToken ct)
        {
            try
            {
                await _service.DeactivateAsync(serviceId, ct);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                return ApiResponseHelper.CreateError("Não encontrado", knf.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar serviço {ServiceId}", serviceId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }
    }
}