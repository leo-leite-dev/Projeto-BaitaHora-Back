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
    public sealed class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("schedules/{scheduleId:guid}/pending")]
        [Authorize(Roles = "Owner,Manager,Staff")]
        public async Task<IActionResult> CreatePending([FromRoute] Guid scheduleId, [FromBody] CreatePendingSlotRequest request, CancellationToken ct)
        {
            try
            {
                var result = await _appointmentService.CreatePendingSlotAsync(scheduleId, request, ct);
                return Ok(ApiResponseHelper.CreateSuccess(result, "Slot pendente criado"));
            }
            catch (InvalidOperationException ioe) 
            {
                return ApiResponseHelper.CreateError("Conflito de horário", ioe.Message, 409);
            }
            catch (ArgumentException ae)
            {
                return ApiResponseHelper.CreateError("Requisição inválida", ae.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar slot pendente na agenda {ScheduleId}", scheduleId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Owner,Manager,Staff,Chatbot")] 
        public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request, CancellationToken ct)
        {
            try
            {
                var result = await _appointmentService.CreateAsync(request, ct);
                return Ok(ApiResponseHelper.CreateSuccess(result, "Agendamento criado"));
            }
            catch (KeyNotFoundException knf)
            {
                return ApiResponseHelper.CreateError("Não encontrado", knf.Message, 404);
            }
            catch (InvalidOperationException ioe)
            {
                return ApiResponseHelper.CreateError("Conflito de horário", ioe.Message, 409);
            }
            catch (ArgumentException ae)
            {
                return ApiResponseHelper.CreateError("Requisição inválida", ae.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar agendamento");
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpPut("{appointmentId:guid}/assign-customer")]
        [Authorize(Roles = "Owner,Manager,Staff,Chatbot")]
        public async Task<IActionResult> AssignCustomer([FromRoute] Guid appointmentId, [FromBody] AssignCustomerRequest request, CancellationToken ct)
        {
            try
            {
                await _appointmentService.AssignCustomerAsync(appointmentId, request.CustomerId, ct);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                return ApiResponseHelper.CreateError("Não encontrado", knf.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atribuir cliente {CustomerId} ao agendamento {AppointmentId}", request.CustomerId, appointmentId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpPut("{appointmentId:guid}/status")]
        [Authorize(Roles = "Owner,Manager,Staff")]
        public async Task<IActionResult> UpdateStatus([FromRoute] Guid appointmentId, [FromBody] UpdateAppointmentStatusRequest request, CancellationToken ct)
        {
            try
            {
                await _appointmentService.UpdateStatusAsync(appointmentId, request.NewStatus, ct);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                return ApiResponseHelper.CreateError("Não encontrado", knf.Message, 404);
            }
            catch (ArgumentException ae)
            {
                return ApiResponseHelper.CreateError("Requisição inválida", ae.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status do agendamento {AppointmentId}", appointmentId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpPut("{appointmentId:guid}/reschedule")]
        [Authorize(Roles = "Owner,Manager,Staff")]
        public async Task<IActionResult> Reschedule([FromRoute] Guid appointmentId, [FromBody] RescheduleAppointmentRequest request, CancellationToken ct)
        {
            try
            {
                await _appointmentService.RescheduleAsync(appointmentId, request.StartsAtUtc, request.EndsAtUtc, ct);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                return ApiResponseHelper.CreateError("Não encontrado", knf.Message, 404);
            }
            catch (InvalidOperationException ioe)
            {
                return ApiResponseHelper.CreateError("Conflito de horário", ioe.Message, 409);
            }
            catch (ArgumentException ae)
            {
                return ApiResponseHelper.CreateError("Requisição inválida", ae.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao reagendar agendamento {AppointmentId}", appointmentId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }

        [HttpPut("{appointmentId:guid}/cancel")]
        [Authorize(Roles = "Owner,Manager,Staff,Customer")]
        public async Task<IActionResult> Cancel([FromRoute] Guid appointmentId, [FromBody] CancelAppointmentRequest request, CancellationToken ct)
        {
            try
            {
                await _appointmentService.CancelAsync(appointmentId, request.Reason, ct);
                return NoContent();
            }
            catch (KeyNotFoundException knf)
            {
                return ApiResponseHelper.CreateError("Não encontrado", knf.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar agendamento {AppointmentId}", appointmentId);
                return ApiResponseHelper.CreateError("Erro no servidor", "Tente novamente mais tarde", 500);
            }
        }
    }
}