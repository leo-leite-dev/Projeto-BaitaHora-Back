using AutoMapper;
using BaitaHora.Application.DTOs.Commands.Scheduling;
using BaitaHora.Application.DTOs.Responses.Scheduling;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Domain.Entities.Scheduling;
using BaitaHora.Domain.Enums;

public sealed class AppointmentService : IAppointmentService
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IServiceCatalogItemRepository _serviceCatalogItemRepository;
    private readonly IMapper _mapper;

    public AppointmentService(
        IScheduleRepository scheduleRepository,
        IAppointmentRepository appointmentRepository,
        IServiceCatalogItemRepository serviceCatalogItemRepository,
        IMapper mapper)
    {
        _scheduleRepository = scheduleRepository ?? throw new ArgumentNullException(nameof(scheduleRepository));
        _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
        _serviceCatalogItemRepository = serviceCatalogItemRepository ?? throw new ArgumentNullException(nameof(serviceCatalogItemRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AppointmentResponse> CreatePendingSlotAsync(CreatePendingSlotCommand cmd, CancellationToken ct = default)
    {
        if (cmd.DurationMinutes <= 0) throw new ArgumentException("Duração inválida.");
        if (cmd.ScheduleId == Guid.Empty) throw new ArgumentException("ScheduleId inválido.");

        _ = await _scheduleRepository.GetByIdAsync(cmd.ScheduleId, ct)
            ?? throw new KeyNotFoundException("Agenda não encontrada.");

        var starts = cmd.StartsAtUtc;
        var ends = starts.AddMinutes(cmd.DurationMinutes);

        await EnsureNoConflict(cmd.ScheduleId, starts, ends, ignoreId: null, ct);

        var appt = Appointment.CreatePendingSlot(
            scheduleId: cmd.ScheduleId,
            startsAtUtc: starts,
            durationMinutes: cmd.DurationMinutes,
            serviceId: cmd.ServiceId,
            notes: cmd.Notes,
            tentativeName: cmd.TentativeName,
            tentativePhone: cmd.TentativePhone,
            createdBy: AppointmentCreatedBy.Staff);

        await _appointmentRepository.AddAsync(appt, ct);
        return _mapper.Map<AppointmentResponse>(appt);
    }

    public async Task<AppointmentResponse> CreateAsync(CreateAppointmentCommand cmd, CancellationToken ct = default)
    {
        if (cmd.ScheduleId == Guid.Empty) throw new ArgumentException("ScheduleId inválido.");
        if (cmd.EndsAtUtc <= cmd.StartsAtUtc)
            throw new ArgumentException("Horário inválido (fim <= início).");

        _ = await _scheduleRepository.GetByIdAsync(cmd.ScheduleId, ct)
            ?? throw new KeyNotFoundException("Agenda não encontrada.");

        if (cmd.ServiceId.HasValue)
        {
            _ = await _serviceCatalogItemRepository.GetByIdAsync(cmd.ServiceId.Value, ct)
                ?? throw new KeyNotFoundException("Serviço não encontrado.");
        }

        await EnsureNoConflict(cmd.ScheduleId, cmd.StartsAtUtc, cmd.EndsAtUtc, ignoreId: null, ct);

        var appt = Appointment.CreateConfirmed(
            scheduleId: cmd.ScheduleId,
            startsAtUtc: cmd.StartsAtUtc,
            endsAtUtc: cmd.EndsAtUtc,
            createdBy: cmd.CreatedBy,
            serviceId: cmd.ServiceId,
            notes: cmd.Notes,
            customerId: cmd.CustomerId,
            customerDisplayName: cmd.CustomerDisplayName,
            customerPhone: cmd.CustomerPhone
        );

        await _appointmentRepository.AddAsync(appt, ct);
        return _mapper.Map<AppointmentResponse>(appt);
    }

    public async Task AssignCustomerAsync(Guid appointmentId, Guid customerId, CancellationToken ct = default)
    {
        var appt = await _appointmentRepository.GetByIdAsync(appointmentId, ct)
            ?? throw new KeyNotFoundException("Agendamento não encontrado.");

        appt.AssignCustomer(customerId);
        await _appointmentRepository.UpdateAsync(appt, ct);
    }

    public async Task ConfirmAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var appt = await _appointmentRepository.GetByIdAsync(appointmentId, ct)
            ?? throw new KeyNotFoundException("Agendamento não encontrado.");

        appt.Confirm();
        await _appointmentRepository.UpdateAsync(appt, ct);
    }

    public async Task CompleteAsync(Guid appointmentId, CancellationToken ct = default)
    {
        var appt = await _appointmentRepository.GetByIdAsync(appointmentId, ct)
            ?? throw new KeyNotFoundException("Agendamento não encontrado.");

        appt.Complete();
        await _appointmentRepository.UpdateAsync(appt, ct);
    }

    public async Task CancelAsync(Guid appointmentId, string? reason = null, CancellationToken ct = default)
    {
        var appt = await _appointmentRepository.GetByIdAsync(appointmentId, ct)
            ?? throw new KeyNotFoundException("Agendamento não encontrado.");

        appt.Cancel(reason);
        await _appointmentRepository.UpdateAsync(appt, ct);
    }

    public async Task RescheduleAsync(Guid appointmentId, DateTime newStartsAtUtc, DateTime newEndsAtUtc, CancellationToken ct = default)
    {
        if (newEndsAtUtc <= newStartsAtUtc)
            throw new ArgumentException("Horário inválido (fim <= início).");

        var appt = await _appointmentRepository.GetByIdAsync(appointmentId, ct)
            ?? throw new KeyNotFoundException("Agendamento não encontrado.");

        await EnsureNoConflict(appt.ScheduleId, newStartsAtUtc, newEndsAtUtc, ignoreId: appt.Id, ct);

        appt.Reschedule(newStartsAtUtc, newEndsAtUtc);
        await _appointmentRepository.UpdateAsync(appt, ct);
    }

    private async Task EnsureNoConflict(Guid scheduleId, DateTime startsAtUtc, DateTime endsAtUtc, Guid? ignoreId, CancellationToken ct)
    {
        if (await _appointmentRepository.HasConflictAsync(scheduleId, startsAtUtc, endsAtUtc, ignoreId, ct))
            throw new InvalidOperationException("Conflito de horário na agenda.");
    }
}