using BaitaHora.Application.DTOs.Responses.Scheduling;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.IChatbot;
using BaitaHora.Application.IServices.Scheduling;
using BaitaHora.Domain.Entities.Companies.Customers;
using BaitaHora.Domain.Entities.Scheduling;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.Services.Chatbot
{
    public sealed class ChatbotQuickService : IChatbotQuickService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICompanyCustomerRepository _companyCustomersRepository;
        private readonly ICompanyCustomerProfessionalRepository _customerPros;
        private readonly ICompanyMemberRepository _companyMembers;
        private readonly IScheduleRepository _schedules;
        private readonly IAppointmentRepository _appointments;
        private readonly IServiceCatalogItemRepository _services;
        private readonly IUnitOfWork _uow;
        private readonly IScheduleService _scheduleService;

        public ChatbotQuickService(
            ICustomerRepository customerRepository,
            ICompanyCustomerRepository companyCustomersRepository,
            ICompanyCustomerProfessionalRepository customerPros,
            ICompanyMemberRepository companyMembers,
            IScheduleRepository schedules,
            IAppointmentRepository appointments,
            IServiceCatalogItemRepository services,
            IUnitOfWork uow,
            IScheduleService scheduleService)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            _companyCustomersRepository = companyCustomersRepository ?? throw new ArgumentNullException(nameof(companyCustomersRepository));
            _customerPros = customerPros ?? throw new ArgumentNullException(nameof(customerPros));
            _companyMembers = companyMembers ?? throw new ArgumentNullException(nameof(companyMembers));
            _schedules = schedules ?? throw new ArgumentNullException(nameof(schedules));
            _appointments = appointments ?? throw new ArgumentNullException(nameof(appointments));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _scheduleService = scheduleService ?? throw new ArgumentNullException(nameof(scheduleService));
        }

        // --- Cadastro/garantia do Customer + vínculo com a Company ---
        public async Task<Guid> EnsureCustomerUserAsyncMinimal(
            Guid companyId,
            string fullName,
            string phoneE164,
            string? email,
            CancellationToken ct = default)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");
            if (string.IsNullOrWhiteSpace(fullName)) throw new ArgumentException("Nome obrigatório.");
            if (string.IsNullOrWhiteSpace(phoneE164)) throw new ArgumentException("Telefone obrigatório (E.164).");

            var customerId = await _customerRepository.EnsureCustomerMinimalAsync(
                phoneE164.Trim(),
                name: fullName.Trim(),
                email: string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                ct: ct);

            if (!await _companyCustomersRepository.ExistsAsync(companyId, customerId, ct))
                await _companyCustomersRepository.AddAsync(new CompanyCustomer(companyId, customerId), ct);

            await _uow.SaveChangesAsync(ct);
            return customerId;
        }

        // --- FLUXO 1: Criar SLOT PENDENTE (sem cliente) ---
        public async Task<AppointmentResponse> BookPendingSlotAsync(
            Guid companyId,
            DateTime desiredWeekStartUtc,
            Guid? preferredProfessionalUserId = null,
            string? roleName = null,
            Guid? serviceId = null,
            int? overrideDurationMinutes = null,
            CancellationToken ct = default)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");

            TimeSpan duration = TimeSpan.FromMinutes(overrideDurationMinutes ?? 30);
            if (serviceId.HasValue && overrideDurationMinutes is null)
            {
                var svc = await _services.GetByIdAsync(serviceId.Value, ct)
                          ?? throw new KeyNotFoundException("Serviço não encontrado.");
                duration = TimeSpan.FromMinutes(svc.DurationMinutes);
            }

            // Profissional (sem relação com Customer aqui)
            var professionalUserId = await ResolveProfessionalAsync(
                companyId, customerId: Guid.Empty, preferredProfessionalUserId, roleName, ct);

            await _scheduleService.EnsureScheduleAsync(professionalUserId, companyId, ct);

            var schedule = await _schedules.GetByUserAsync(professionalUserId, companyId, ct)
                           ?? throw new KeyNotFoundException("Agenda do profissional não encontrada.");

            var weekStart = DateTime.SpecifyKind(desiredWeekStartUtc.Date, DateTimeKind.Utc);
            var weekEnd = weekStart.AddDays(7);
            var existing = await _appointments.GetByScheduleAsync(schedule.Id, weekStart, weekEnd, ct);

            DateTime candidate = weekStart.AddHours(9);
            DateTime limit = weekEnd.AddHours(18);

            await _uow.BeginTransactionAsync();
            try
            {
                while (candidate.Add(duration) <= limit)
                {
                    var candidateEnd = candidate.Add(duration);

                    bool conflict = existing.Any(a =>
                        a.Status != AppointmentStatus.Cancelled &&
                        a.EndsAtUtc > candidate &&
                        a.StartsAtUtc < candidateEnd);

                    if (!conflict)
                    {
                        var appt = Appointment.CreatePendingSlot(
                            scheduleId: schedule.Id,
                            startsAtUtc: candidate,
                            durationMinutes: (int)duration.TotalMinutes,
                            serviceId: serviceId,
                            notes: "Criado pelo chatbot (slot pendente)",
                            createdBy: AppointmentCreatedBy.Chatbot
                        );

                        await _appointments.AddAsync(appt, ct);
                        await _uow.CommitAsync();

                        return Map(appt);
                    }

                    candidate = NextSlot(candidate, stepMinutes: 30);
                    if (candidate.Hour >= 18) candidate = NextBusinessDay(candidate, hour: 9);
                }

                throw new InvalidOperationException("Não há horários disponíveis nesta semana.");
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        // --- FLUXO 2: Criar AGENDAMENTO CONFIRMADO (com cliente) ---
        public async Task<AppointmentResponse> BookConfirmedAsync(
            Guid companyId,
            Guid customerId,
            DateTime desiredWeekStartUtc,
            Guid? preferredProfessionalUserId = null,
            string? roleName = null,
            Guid? serviceId = null,
            int? overrideDurationMinutes = null,
            CancellationToken ct = default)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");
            if (customerId == Guid.Empty) throw new ArgumentException("CustomerId inválido.");

            TimeSpan duration = TimeSpan.FromMinutes(overrideDurationMinutes ?? 30);
            if (serviceId.HasValue && overrideDurationMinutes is null)
            {
                var svc = await _services.GetByIdAsync(serviceId.Value, ct)
                          ?? throw new KeyNotFoundException("Serviço não encontrado.");
                duration = TimeSpan.FromMinutes(svc.DurationMinutes);
            }

            var professionalUserId = await ResolveProfessionalAsync(
                companyId, customerId, preferredProfessionalUserId, roleName, ct);

            await _scheduleService.EnsureScheduleAsync(professionalUserId, companyId, ct);

            var schedule = await _schedules.GetByUserAsync(professionalUserId, companyId, ct)
                           ?? throw new KeyNotFoundException("Agenda do profissional não encontrada.");

            var weekStart = DateTime.SpecifyKind(desiredWeekStartUtc.Date, DateTimeKind.Utc);
            var weekEnd = weekStart.AddDays(7);
            var existing = await _appointments.GetByScheduleAsync(schedule.Id, weekStart, weekEnd, ct);

            DateTime candidate = weekStart.AddHours(9);
            DateTime limit = weekEnd.AddHours(18);

            await _uow.BeginTransactionAsync();
            try
            {
                while (candidate.Add(duration) <= limit)
                {
                    var candidateEnd = candidate.Add(duration);

                    bool conflict = existing.Any(a =>
                        a.Status != AppointmentStatus.Cancelled &&
                        a.EndsAtUtc > candidate &&
                        a.StartsAtUtc < candidateEnd);

                    if (!conflict)
                    {
                        var appt = Appointment.CreateConfirmed(
                            scheduleId: schedule.Id,
                            startsAtUtc: candidate,
                            endsAtUtc: candidateEnd,
                            createdBy: AppointmentCreatedBy.Chatbot,
                            serviceId: serviceId,
                            notes: "Criado pelo chatbot (confirmado)",
                            customerId: customerId,
                            customerDisplayName: null,
                            customerPhone: null
                        );

                        await _appointments.AddAsync(appt, ct);

                        // Atualiza vínculos (agora temos cliente)
                        await UpsertCompanyCustomerLinksAsync(
                            companyId, customerId, professionalUserId, serviceId, ct);

                        await _uow.CommitAsync();
                        return Map(appt);
                    }

                    candidate = NextSlot(candidate, stepMinutes: 30);
                    if (candidate.Hour >= 18) candidate = NextBusinessDay(candidate, hour: 9);
                }

                throw new InvalidOperationException("Não há horários disponíveis nesta semana.");
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        // --- PASSO 2 do fluxo pendente: atribuir cliente a um slot e confirmar ---
        public async Task<AppointmentResponse> AssignCustomerToAppointmentAsync(
            Guid companyId,
            Guid appointmentId,
            Guid customerId,
            Guid? serviceId = null,
            CancellationToken ct = default)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");
            if (appointmentId == Guid.Empty) throw new ArgumentException("AppointmentId inválido.");
            if (customerId == Guid.Empty) throw new ArgumentException("CustomerId inválido.");

            await _uow.BeginTransactionAsync();
            try
            {
                var appt = await _appointments.GetByIdAsync(appointmentId, ct)
                           ?? throw new KeyNotFoundException("Agendamento não encontrado.");

                // Obter profissional pela agenda
                var schedule = await _schedules.GetByIdAsync(appt.ScheduleId, ct)
                               ?? throw new KeyNotFoundException("Agenda do profissional não encontrada.");
                var professionalUserId = schedule.UserId;

                // Vincula cliente (muda Pending -> Confirmed)
                appt.AssignCustomer(customerId);
                if (serviceId.HasValue)
                    appt.SetService(serviceId);

                await UpsertCompanyCustomerLinksAsync(
                    companyId, customerId, professionalUserId, serviceId, ct);

                await _uow.SaveChangesAsync(ct);
                await _uow.CommitAsync();

                return Map(appt);
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        // --- Legacy (mantido, se quiser) ---
        public Task<AppointmentResponse> BookForCustomerLegacyAsync(
            Guid companyId,
            Guid customerUserId,
            DateTime desiredWeekStartUtc,
            Guid? preferredProfessionalUserId = null,
            string? roleName = null,
            Guid? serviceId = null,
            CancellationToken ct = default)
        {
            // Chama o fluxo "Confirmado"
            return BookConfirmedAsync(
                companyId,
                customerUserId,
                desiredWeekStartUtc,
                preferredProfessionalUserId,
                roleName,
                serviceId,
                overrideDurationMinutes: null,
                ct);
        }

        // ----------------- Helpers -----------------
        private static AppointmentResponse Map(Appointment appt) => new AppointmentResponse
        {
            Id = appt.Id,
            ScheduleId = appt.ScheduleId,
            StartsAtUtc = new DateTimeOffset(DateTime.SpecifyKind(appt.StartsAtUtc, DateTimeKind.Utc)),
            EndsAtUtc = new DateTimeOffset(DateTime.SpecifyKind(appt.EndsAtUtc, DateTimeKind.Utc)),
            Status = appt.Status.ToString(),
            CreatedBy = appt.CreatedBy.ToString(),
            ServiceId = appt.ServiceId,
            CustomerId = appt.CustomerId,
            CustomerDisplayName = appt.CustomerDisplayName,
            CustomerPhone = appt.CustomerPhone,
            Notes = appt.Notes
        };

        private static DateTime NextSlot(DateTime current, int stepMinutes)
            => current.AddMinutes(stepMinutes);

        private static DateTime NextBusinessDay(DateTime current, int hour)
            => current.Date.AddDays(1).AddHours(hour);

        private async Task UpsertCompanyCustomerLinksAsync(
            Guid companyId,
            Guid customerId,
            Guid professionalUserId,
            Guid? serviceId,
            CancellationToken ct)
        {
            var cc = await _companyCustomersRepository.GetAsync(companyId, customerId, ct);
            if (cc is not null)
            {
                cc.SetPreferredProfessional(professionalUserId);
                cc.TouchLastVisit(serviceId, professionalUserId, DateTime.UtcNow);
                _companyCustomersRepository.Update(cc);   // << sem await
                await _uow.CommitAsync(ct);
            }
            else
            {
                var link = new CompanyCustomer(companyId, customerId);
                link.SetPreferredProfessional(professionalUserId);
                link.TouchLastVisit(serviceId, professionalUserId, DateTime.UtcNow);
                await _companyCustomersRepository.AddAsync(link, ct);
            }

            var hasLink = await _customerPros.ExistsAsync(companyId, customerId, professionalUserId, ct);
            if (!hasLink)
            {
                await _customerPros.AddAsync(
                    new CompanyCustomerProfessional(companyId, customerId, professionalUserId, isPrimary: true),
                    ct);
            }
        }

        private async Task<Guid> ResolveProfessionalAsync(
            Guid companyId,
            Guid customerId,
            Guid? preferredProfessionalUserId,
            string? roleName,
            CancellationToken ct)
        {
            if (preferredProfessionalUserId.HasValue)
                return preferredProfessionalUserId.Value;

            if (customerId != Guid.Empty)
            {
                var cc = await _companyCustomersRepository.GetAsync(companyId, customerId, ct);
                if (cc?.PreferredProfessionalUserId is Guid pref && pref != Guid.Empty)
                    return pref;

                var primary = await _customerPros.GetPrimaryAsync(companyId, customerId, ct);
                if (primary is not null)
                    return primary.ProfessionalUserId;
            }

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var member = await _companyMembers.FindAnyActiveByPositionNameAsync(companyId, roleName.Trim(), ct);
                if (member is not null)
                    return member.UserId;
            }

            var any = await _companyMembers.FindAnyActiveAsync(companyId, ct)
                      ?? throw new InvalidOperationException("Nenhum profissional ativo encontrado.");

            return any.UserId;
        }
    }
}