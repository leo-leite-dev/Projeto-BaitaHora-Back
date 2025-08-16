using BaitaHora.Domain.Entities.Commons;
using BaitaHora.Domain.Entities.Companies.Customers;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Domain.Entities.Scheduling
{
    public class Appointment : Base
    {
        public Guid ScheduleId { get; private set; }
        public Schedule Schedule { get; private set; } = null!;

        public DateTime StartsAtUtc { get; private set; }
        public DateTime EndsAtUtc { get; private set; }

        public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;
        public AppointmentCreatedBy CreatedBy { get; private set; } = AppointmentCreatedBy.Staff;

        public Guid? CustomerId { get; private set; }
        public Customer? Customer { get; private set; }

        public string? CustomerDisplayName { get; private set; }
        public string? CustomerPhone { get; private set; }

        public Guid? ServiceId { get; private set; }

        public string? Notes { get; private set; }
        public string? CancellationReason { get; private set; }

        public byte[]? RowVersion { get; private set; }

        private Appointment() { }

        private Appointment(Guid scheduleId, DateTime startsAtUtc, DateTime endsAtUtc, AppointmentCreatedBy createdBy, Guid? serviceId,
            string? notes, string? customerDisplayName, string? customerPhone, AppointmentStatus initialStatus)
        {
            if (scheduleId == Guid.Empty) throw new ArgumentException("ScheduleId inválido.", nameof(scheduleId));
            EnsureTimeRange(startsAtUtc, endsAtUtc);

            ScheduleId = scheduleId;
            StartsAtUtc = startsAtUtc;
            EndsAtUtc = endsAtUtc;
            CreatedBy = createdBy;
            ServiceId = serviceId;
            Notes = Normalize(notes);
            CustomerDisplayName = Normalize(customerDisplayName);
            CustomerPhone = Normalize(customerPhone);
            Status = initialStatus;
        }

        public static Appointment CreatePendingSlot(Guid scheduleId, DateTime startsAtUtc, int durationMinutes, Guid? serviceId = null,
            string? notes = null, string? tentativeName = null, string? tentativePhone = null, AppointmentCreatedBy createdBy = AppointmentCreatedBy.Staff)
        {
            if (durationMinutes <= 0) throw new ArgumentException("Duração inválida.", nameof(durationMinutes));
            var ends = startsAtUtc.AddMinutes(durationMinutes);

            return new Appointment(scheduleId, startsAtUtc, ends, createdBy, serviceId, notes, tentativeName, tentativePhone, AppointmentStatus.Pending
            );
        }

        public static Appointment CreateConfirmed(Guid scheduleId, DateTime startsAtUtc, DateTime endsAtUtc, AppointmentCreatedBy createdBy, Guid? serviceId,
            string? notes, Guid? customerId, string? customerDisplayName, string? customerPhone)
        {
            var appt = new Appointment(scheduleId, startsAtUtc, endsAtUtc, createdBy, serviceId, notes, customerDisplayName, customerPhone,
                customerId.HasValue ? AppointmentStatus.Confirmed : AppointmentStatus.Pending
            );

            if (customerId.HasValue)
                appt.AssignCustomer(customerId.Value);

            return appt;
        }

        public void AssignCustomer(Guid customerId)
        {
            if (customerId == Guid.Empty) throw new ArgumentException("CustomerId inválido.", nameof(customerId));
            EnsureNotFinalized();

            CustomerId = customerId;
            CustomerDisplayName = null;
            CustomerPhone = null;

            if (Status == AppointmentStatus.Pending)
                Status = AppointmentStatus.Confirmed;

            Touch();
        }

        public void AssignAnonymousCustomer(string? displayName, string? phoneE164)
        {
            EnsureNotFinalized();

            CustomerId = null;
            CustomerDisplayName = Normalize(displayName);
            CustomerPhone = Normalize(phoneE164);

            Touch();
        }

        public void SetService(Guid? serviceId)
        {
            EnsureNotFinalized();
            ServiceId = serviceId;
            Touch();
        }

        public void Reschedule(DateTime newStartsAtUtc, DateTime newEndsAtUtc)
        {
            EnsureNotFinalized();
            EnsureTimeRange(newStartsAtUtc, newEndsAtUtc);

            StartsAtUtc = newStartsAtUtc;
            EndsAtUtc = newEndsAtUtc;
            Touch();
        }

        public void Confirm()
        {
            EnsureNotFinalized();
            if (Status == AppointmentStatus.Pending)
            {
                Status = AppointmentStatus.Confirmed;
                Touch();
            }
        }

        public void Complete()
        {
            EnsureNotFinalized();
            Status = AppointmentStatus.Completed;
            Touch();
        }

        public void Cancel(string? reason = null)
        {
            if (Status == AppointmentStatus.Cancelled) return;
            EnsureNotCompleted();

            Status = AppointmentStatus.Cancelled;
            CancellationReason = Normalize(reason);
            Touch();
        }

        public void UpdateNotes(string? notes)
        {
            EnsureNotFinalized();
            Notes = Normalize(notes);
            Touch();
        }

        private static void EnsureTimeRange(DateTime startsAtUtc, DateTime endsAtUtc)
        {
            if (endsAtUtc <= startsAtUtc) throw new ArgumentException("Horário inválido (fim <= início).");
        }

        private static string? Normalize(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private void EnsureNotFinalized()
        {
            if (Status is AppointmentStatus.Cancelled or AppointmentStatus.Completed)
                throw new InvalidOperationException("Operação não permitida: agendamento finalizado.");
        }

        private void EnsureNotCompleted()
        {
            if (Status is AppointmentStatus.Completed)
                throw new InvalidOperationException("Operação não permitida: agendamento já concluído.");
        }
        private void Touch() => base.Touch();
    }
}