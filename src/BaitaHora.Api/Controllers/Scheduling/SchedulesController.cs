using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaitaHora.Api.Helpers;
using BaitaHora.Application.IServices.Scheduling;

namespace BaitaHora.Api.Controllers.Scheduling
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public sealed class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<SchedulesController> _logger;

        public SchedulesController(IScheduleService scheduleService, ILogger<SchedulesController> logger)
        {
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("{companyId:guid}/users/{userId:guid}/ensure")]
        public async Task<IActionResult> EnsureSchedule([FromRoute] Guid companyId, [FromRoute] Guid userId, CancellationToken ct)
        {
            try
            {
                var schedule = await _scheduleService.EnsureScheduleAsync(userId, companyId, ct);
                return Ok(ApiResponseHelper.CreateSuccess(schedule, "Agenda garantida"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao garantir agenda para user {UserId} company {CompanyId}", userId, companyId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpGet("{companyId:guid}/users/{userId:guid}/appointments")]
        public async Task<IActionResult> GetAppointments([FromRoute] Guid companyId, [FromRoute] Guid userId,
            [FromQuery] DateTime fromUtc, [FromQuery] DateTime toUtc, CancellationToken ct)
        {
            if (toUtc <= fromUtc)
                return ApiResponseHelper.CreateError("Intervalo inválido", "toUtc deve ser maior que fromUtc", 400);

            try
            {
                var items = await _scheduleService.GetAppointmentsAsync(userId, companyId, fromUtc, toUtc, ct);
                return Ok(ApiResponseHelper.CreateSuccess(items, "Agendamentos carregados"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter agendamentos para user {UserId} company {CompanyId}", userId, companyId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpDelete("{scheduleId:guid}")]
        [Authorize(Roles = "Owner,Manager")]
        public async Task<IActionResult> Deactivate([FromRoute] Guid scheduleId, CancellationToken ct)
        {
            try
            {
                await _scheduleService.DeactivateAsync(scheduleId, ct);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                return ApiResponseHelper.CreateError("Não encontrado", knf.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar agenda {ScheduleId}", scheduleId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }
    }
}