using BaitaHora.Application.DTOs.Responses.Scheduling;
using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.IChatbot;
using BaitaHora.Application.IServices.Scheduling; // <- para EnsureScheduleAsync (opcional mas recomendado)
using BaitaHora.Domain.Entities;
using BaitaHora.Domain.Entities.Customers;

namespace BaitaHora.Application.Services.Chatbot
{
    public sealed class ChatbotQuickService : IChatbotQuickService
    {
        private readonly ICustomerRepository _customers;
        private readonly ICompanyCustomerRepository _companyCustomers;
        private readonly ICompanyCustomerProfessionalRepository _customerPros;
        private readonly ICompanyMemberRepository _companyMembers;
        private readonly IScheduleRepository _schedules;
        private readonly IAppointmentRepository _appointments;
        private readonly IServiceCatalogItemRepository _services;
        private readonly IUnitOfWork _uow;
        private readonly IScheduleService _scheduleService; // garante agenda do profissional

        public ChatbotQuickService(
            ICustomerRepository customers,
            ICompanyCustomerRepository companyCustomers,
            ICompanyCustomerProfessionalRepository customerPros,
            ICompanyMemberRepository companyMembers,
            IScheduleRepository schedules,
            IAppointmentRepository appointments,
            IServiceCatalogItemRepository services,
            IUnitOfWork uow,
            IScheduleService scheduleService)
        {
            _customers = customers;
            _companyCustomers = companyCustomers;
            _customerPros = customerPros;
            _companyMembers = companyMembers;
            _schedules = schedules;
            _appointments = appointments;
            _services = services;
            _uow = uow;
            _scheduleService = scheduleService;
        }

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

            // Normalização mínima aqui; regras mais pesadas podem morar no repo
            var customerId = await _customers.EnsureCustomerMinimalAsync(
                phoneE164.Trim(),
                name: fullName.Trim(),
                email: string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
                ct: ct);

            // Vincula à company se ainda não existir
            if (!await _companyCustomers.ExistsAsync(companyId, customerId, ct))
            {
                await _companyCustomers.AddAsync(new CompanyCustomer(companyId, customerId));
            }

            // Persistência simples: sem transação por não haver múltiplas escritas correlacionadas
            await _uow.CommitAsync();

            return customerId;
        }

        public async Task<AppointmentResponse> BookForCustomerAsync(
            Guid companyId,
            Guid customerId,
            DateTime desiredWeekStartUtc,
            Guid? preferredProfessionalUserId = null,
            string? roleName = null,
            Guid? serviceId = null,
            CancellationToken ct = default)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");
            if (customerId == Guid.Empty) throw new ArgumentException("CustomerId inválido.");

            // Duração padrão
            TimeSpan duration = TimeSpan.FromMinutes(30);
            if (serviceId.HasValue)
            {
                var svc = await _services.GetByIdAsync(serviceId.Value);
                if (svc is null) throw new KeyNotFoundException("Serviço não encontrado.");
                duration = TimeSpan.FromMinutes(svc.DurationMinutes);
            }

            // Resolve profissional (preferência do cliente / papel / qualquer ativo)
            var professionalUserId = await ResolveProfessionalAsync(
                companyId, customerId, preferredProfessionalUserId, roleName, ct);

            // Garante que há agenda para esse profissional nessa company
            await _scheduleService.EnsureScheduleAsync(professionalUserId, companyId);

            // Busca agenda
            var schedule = await _schedules.GetByUserAsync(professionalUserId, companyId, ct)
                           ?? throw new KeyNotFoundException("Agenda do profissional não encontrada.");

            // Janela semanal
            var weekStart = DateTime.SpecifyKind(desiredWeekStartUtc.Date, DateTimeKind.Utc);
            var weekEnd = weekStart.AddDays(7);
            var existing = await _appointments.GetByScheduleAsync(schedule.Id, weekStart, weekEnd, ct);

            // Janela de tentativa (09:00–18:00, steps de 30 min)
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
                        var appt = new Appointment(
                            schedule.Id,
                            candidate,
                            candidateEnd,
                            AppointmentCreatedBy.Chatbot,
                            serviceId,
                            notes: "Criado pelo chatbot");

                        appt.AssignCustomer(customerId);
                        await _appointments.AddAsync(appt);

                        // Atualiza CompanyCustomer (preferências & last visit)
                        var cc = await _companyCustomers.GetAsync(companyId, customerId, ct);
                        if (cc is not null)
                        {
                            cc.SetPreferredProfessional(professionalUserId);
                            cc.TouchLastVisit(serviceId, professionalUserId, DateTime.UtcNow);
                            await _companyCustomers.UpdateAsync(cc);
                        }
                        else
                        {
                            // Se por algum motivo ainda não está vinculado, cria o vínculo agora
                            var link = new CompanyCustomer(companyId, customerId);
                            link.SetPreferredProfessional(professionalUserId);
                            link.TouchLastVisit(serviceId, professionalUserId, DateTime.UtcNow);
                            await _companyCustomers.AddAsync(link);
                        }

                        // Garante o vínculo CompanyCustomer x Professional (primário)
                        var hasLink = await _customerPros.ExistsAsync(companyId, customerId, professionalUserId, ct);
                        if (!hasLink)
                        {
                            await _customerPros.AddAsync(
                                new CompanyCustomerProfessional(companyId, customerId, professionalUserId, isPrimary: true));
                        }

                        await _uow.CommitAsync();

                        return new AppointmentResponse
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
                    }

                    // Próximo slot
                    candidate = candidate.AddMinutes(30);
                    if (candidate.Hour >= 18) candidate = candidate.Date.AddDays(1).AddHours(9);
                }

                throw new InvalidOperationException("Não há horários disponíveis nesta semana.");
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }

        public Task<AppointmentResponse> BookForCustomerLegacyAsync(
            Guid companyId,
            Guid customerUserId,
            DateTime desiredWeekStartUtc,
            Guid? preferredProfessionalUserId = null,
            string? roleName = null,
            Guid? serviceId = null,
            CancellationToken ct = default)
        {
            return BookForCustomerAsync(companyId, customerUserId, desiredWeekStartUtc,
                preferredProfessionalUserId, roleName, serviceId, ct);
        }

        private async Task<Guid> ResolveProfessionalAsync(
            Guid companyId,
            Guid customerId,
            Guid? preferredProfessionalUserId,
            string? roleName, // aqui "roleName" na verdade é o NOME DO CARGO (ex.: "manicure")
            CancellationToken ct)
        {
            if (preferredProfessionalUserId.HasValue)
                return preferredProfessionalUserId.Value;

            var cc = await _companyCustomers.GetAsync(companyId, customerId, ct);
            if (cc?.PreferredProfessionalUserId is Guid pref && pref != Guid.Empty)
                return pref;

            var primary = await _customerPros.GetPrimaryAsync(companyId, customerId, ct);
            if (primary is not null)
                return primary.ProfessionalUserId;

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                // usa o método que você JÁ tem no repo: busca por NOME DO CARGO
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