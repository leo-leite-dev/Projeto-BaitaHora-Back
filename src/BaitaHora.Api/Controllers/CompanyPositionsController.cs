using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Helpers;
using BaitaHora.Application.DTOs.Requests.Company;
using BaitaHora.Api.Extensions;
using BaitaHora.Application.IServices.Companies;

namespace BaitaHora.Api.Controllers
{
    [ApiController]
    [Route("api/companies/{companyId:guid}/positions")]
    [Authorize]
    public sealed class CompanyPositionsController : ControllerBase
    {
        private readonly ICompanyPositionService _companyPositionService;
        private readonly ILogger<CompanyPositionsController> _logger;

        public CompanyPositionsController(
            ICompanyPositionService companyPositionService,
            ILogger<CompanyPositionsController> logger)
        {
            _companyPositionService = companyPositionService ?? throw new ArgumentNullException(nameof(companyPositionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromRoute] Guid companyId,
            [FromBody] CompanyPositionRequest body,
            CancellationToken ct)
        {
            if (body is null)
                return ApiResponseHelper.CreateError("Requisição inválida", "Body obrigatório.", 400);

            var requesterId = User.GetUserId();
            if (requesterId == Guid.Empty)
                return ApiResponseHelper.CreateError("Não autenticado", "Token inválido.", 401);

            try
            {
                var id = await _companyPositionService.CreateAsync(companyId, requesterId, body.Name, body.AccessLevel, ct);
                return Ok(ApiResponseHelper.CreateSuccess(new { id }, "Cargo criado com sucesso."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return ApiResponseHelper.CreateError("Sem permissão", ex.Message, 403);
            }
            catch (KeyNotFoundException ex)
            {
                return ApiResponseHelper.CreateError("Não encontrado", ex.Message, 404);
            }
            catch (ArgumentException ex)
            {
                return ApiResponseHelper.CreateError("Requisição inválida", ex.Message, 400);
            }
            catch (InvalidOperationException ex)
            {
                return ApiResponseHelper.CreateError("Conflito", ex.Message, 409);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cargo para company {CompanyId}", companyId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }
    }
}