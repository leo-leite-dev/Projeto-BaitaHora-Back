using AutoMapper;
using BaitaHora.Application.DTOs.Responses.Scheduling;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Domain.Entities.Scheduling;

namespace BaitaHora.Application.Services.Scheduling
{
    public sealed class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _schedulesRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public ScheduleService(
            IScheduleRepository schedulesRepository,
            IAppointmentRepository appointmentRepository,
            IMapper mapper)
        {
            _schedulesRepository = schedulesRepository ?? throw new ArgumentNullException(nameof(schedulesRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ScheduleResponse> EnsureScheduleAsync(Guid userId, Guid companyId, CancellationToken ct = default)
        {
            var existing = await _schedulesRepository.GetByUserAsync(userId, companyId, ct);
            if (existing != null)
                return _mapper.Map<ScheduleResponse>(existing);

            var created = new Schedule(userId, companyId);
            await _schedulesRepository.AddAsync(created, ct);
            return _mapper.Map<ScheduleResponse>(created);
        }

        public async Task<ScheduleResponse?> GetByUserAsync(Guid userId, Guid companyId, CancellationToken ct = default)
        {
            var s = await _schedulesRepository.GetByUserAsync(userId, companyId, ct);
            return s is null ? null : _mapper.Map<ScheduleResponse>(s);
        }

        public async Task<IReadOnlyList<AppointmentResponse>> GetAppointmentsAsync(
            Guid userId, Guid companyId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default)
        {
            var schedule = await _schedulesRepository.GetByUserAsync(userId, companyId, ct);
            if (schedule is null) return Array.Empty<AppointmentResponse>();

            var list = await _appointmentRepository.GetByScheduleAsync(schedule.Id, fromUtc, toUtc, ct);
            return list.Select(x => _mapper.Map<AppointmentResponse>(x)).ToList();
        }

        public async Task DeactivateAsync(Guid scheduleId, CancellationToken ct = default)
        {
            var s = await _schedulesRepository.GetByIdAsync(scheduleId, ct)
                ?? throw new KeyNotFoundException("Agenda não encontrada.");

            s.Deactivate();
            await _schedulesRepository.UpdateAsync(s, ct);
        }
    }
}