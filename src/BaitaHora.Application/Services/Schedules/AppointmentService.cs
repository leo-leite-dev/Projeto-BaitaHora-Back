using AutoMapper;
using BaitaHora.Application.DTOs.Requests.Scheduling;
using BaitaHora.Application.DTOs.Responses.Scheduling;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.Services.Scheduling
{
    public sealed class AppointmentService : IAppointmentService
    {
        private readonly IScheduleRepository _schedules;
        private readonly IAppointmentRepository _appointments;
        private readonly IServiceCatalogItemRepository _services;
        private readonly IMapper _mapper;

        public AppointmentService(
            IScheduleRepository schedules,
            IAppointmentRepository appointments,
            IServiceCatalogItemRepository services,
            IMapper mapper)
        {
            _schedules = schedules ?? throw new ArgumentNullException(nameof(schedules));
            _appointments = appointments ?? throw new ArgumentNullException(nameof(appointments));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<AppointmentResponse> CreatePendingSlotAsync(Guid scheduleId, CreatePendingSlotRequest request, CancellationToken ct = default)
        {
            if (request.DurationMinutes <= 0) throw new ArgumentException("Duração inválida.");

            var starts = request.StartsAtUtc;
            var ends = starts.AddMinutes(request.DurationMinutes);

            await EnsureNoConflict(scheduleId, starts, ends, null, ct);

            var appt = new Appointment(scheduleId, starts, ends, AppointmentCreatedBy.Staff, request.ServiceId, request.Notes, request.TentativeName, request.TentativePhone);

            await _appointments.AddAsync(appt);
            return _mapper.Map<AppointmentResponse>(appt);
        }

        public async Task<AppointmentResponse> CreateAsync(CreateAppointmentRequest request, CancellationToken ct = default)
        {
            if (request.EndsAtUtc <= request.StartsAtUtc)
                throw new ArgumentException("Horário inválido (fim <= início).");

            _ = await _schedules.GetByIdAsync(request.ScheduleId)
                ?? throw new KeyNotFoundException("Agenda não encontrada.");

            if (request.ServiceId.HasValue)
            {
                _ = await _services.GetByIdAsync(request.ServiceId.Value)
                    ?? throw new KeyNotFoundException("Serviço não encontrado.");
            }

            await EnsureNoConflict(request.ScheduleId, request.StartsAtUtc, request.EndsAtUtc, null, ct);

            var createdBy = ParseCreatedBy(request.CreatedBy);

            var appt = new Appointment(request.ScheduleId, request.StartsAtUtc, request.EndsAtUtc, createdBy, request.ServiceId, request.Notes, request.CustomerDisplayName, request.CustomerPhone);

            if (request.CustomerUserId.HasValue)
                appt.AssignCustomer(request.CustomerUserId.Value);

            await _appointments.AddAsync(appt);
            return _mapper.Map<AppointmentResponse>(appt);
        }

        public async Task AssignCustomerAsync(Guid appointmentId, Guid customerUserId, CancellationToken ct = default)
        {
            var appt = await _appointments.GetByIdAsync(appointmentId) ?? throw new KeyNotFoundException("Agendamento não encontrado.");
            appt.AssignCustomer(customerUserId);
            await _appointments.UpdateAsync(appt);
        }

        public async Task UpdateStatusAsync(Guid appointmentId, string newStatus, CancellationToken ct = default)
        {
            var appt = await _appointments.GetByIdAsync(appointmentId) ?? throw new KeyNotFoundException("Agendamento não encontrado.");
            var status = ParseStatus(newStatus);
            appt.UpdateStatus(status);
            await _appointments.UpdateAsync(appt);
        }

        public async Task RescheduleAsync(Guid appointmentId, DateTime newStartsAtUtc, DateTime newEndsAtUtc, CancellationToken ct = default)
        {
            if (newEndsAtUtc <= newStartsAtUtc)
                throw new ArgumentException("Horário inválido (fim <= início).");

            var appt = await _appointments.GetByIdAsync(appointmentId) ?? throw new KeyNotFoundException("Agendamento não encontrado.");

            await EnsureNoConflict(appt.ScheduleId, newStartsAtUtc, newEndsAtUtc, appt.Id, ct);

            appt.Reschedule(newStartsAtUtc, newEndsAtUtc);

            await _appointments.UpdateAsync(appt);
        }

        public async Task CancelAsync(Guid appointmentId, string? reason = null, CancellationToken ct = default)
        {
            var appt = await _appointments.GetByIdAsync(appointmentId) ?? throw new KeyNotFoundException("Agendamento não encontrado.");

            appt.UpdateStatus(AppointmentStatus.Cancelled);

            if (!string.IsNullOrWhiteSpace(reason))
            {
                appt.UpdateNotes(
                    string.IsNullOrWhiteSpace(appt.Notes)
                        ? reason.Trim()
                        : $"{appt.Notes}\nCancel reason: {reason.Trim()}");
            }

            await _appointments.UpdateAsync(appt);
        }

        private static AppointmentStatus ParseStatus(string input)
        {
            if (Enum.TryParse<AppointmentStatus>(input, true, out var status)) return status;
            throw new ArgumentException($"Status inválido: {input}");
        }

        private static AppointmentCreatedBy ParseCreatedBy(string input)
        {
            if (Enum.TryParse<AppointmentCreatedBy>(input, true, out var createdBy)) return createdBy;
            throw new ArgumentException($"CreatedBy inválido: {input}");
        }

        private async Task EnsureNoConflict(Guid scheduleId, DateTime startsAtUtc, DateTime endsAtUtc, Guid? ignoreId, CancellationToken ct)
        {
            if (await _appointments.HasConflictAsync(scheduleId, startsAtUtc, endsAtUtc, ignoreId, ct))
                throw new InvalidOperationException("Conflito de horário na agenda.");
        }
    }
}