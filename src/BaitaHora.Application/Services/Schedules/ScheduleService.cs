using AutoMapper;
using BaitaHora.Application.DTOs.Responses.Scheduling;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.Services.Scheduling
{
    public sealed class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _schedules;
        private readonly IAppointmentRepository _appointments;
        private readonly IMapper _mapper;

        public ScheduleService(
            IScheduleRepository schedules,
            IAppointmentRepository appointments,
            IMapper mapper)
        {
            _schedules = schedules ?? throw new ArgumentNullException(nameof(schedules));
            _appointments = appointments ?? throw new ArgumentNullException(nameof(appointments));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ScheduleResponse> EnsureScheduleAsync(Guid userId, Guid companyId, CancellationToken ct = default)
        {
            var existing = await _schedules.GetByUserAsync(userId, companyId, ct);
            if (existing is not null) return _mapper.Map<ScheduleResponse>(existing);

            var created = new Schedule(userId, companyId);
            await _schedules.AddAsync(created);
            return _mapper.Map<ScheduleResponse>(created);
        }

        public async Task<ScheduleResponse?> GetByUserAsync(Guid userId, Guid companyId, CancellationToken ct = default)
        {
            var s = await _schedules.GetByUserAsync(userId, companyId, ct);
            return s is null ? null : _mapper.Map<ScheduleResponse>(s);
        }

        public async Task<IReadOnlyList<AppointmentResponse>> GetAppointmentsAsync(Guid userId, Guid companyId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            var schedule = await _schedules.GetByUserAsync(userId, companyId, ct);
            if (schedule is null) return Array.Empty<AppointmentResponse>();

            var list = await _appointments.GetByScheduleAsync(schedule.Id, fromUtc, toUtc, ct);
            return list.Select(_mapper.Map<AppointmentResponse>).ToList();
        }

        public async Task DeactivateAsync(Guid scheduleId, CancellationToken ct = default)
        {
            var s = await _schedules.GetByIdAsync(scheduleId) ?? throw new KeyNotFoundException("Agenda não encontrada.");
            s.Deactivate();
            await _schedules.UpdateAsync(s);
        }
    }
}