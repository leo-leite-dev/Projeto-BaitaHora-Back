namespace BaitaHora.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid ScheduleId { get; private set; }
        public DateTime StartsAtUtc { get; private set; }
        public DateTime EndsAtUtc { get; private set; }
        public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;

        public Guid? CustomerUserId { get; private set; }
        public string? CustomerDisplayName { get; private set; }
        public string? CustomerPhone { get; private set; }

        public Guid? ServiceId { get; private set; }
        public AppointmentCreatedBy CreatedBy { get; private set; } = AppointmentCreatedBy.Staff;
        public string? Notes { get; private set; }

        public Schedule Schedule { get; private set; } = null!;
        public User? CustomerUser { get; private set; }

        private Appointment() { }

        public Appointment(Guid scheduleId, DateTime startsAtUtc, DateTime endsAtUtc, AppointmentCreatedBy createdBy,
                           Guid? serviceId = null, string? notes = null,
                           string? customerDisplayName = null, string? customerPhone = null)
        {
            if (scheduleId == Guid.Empty) throw new ArgumentException("ScheduleId inválido.");
            if (endsAtUtc <= startsAtUtc) throw new ArgumentException("Horário inválido.");

            ScheduleId = scheduleId;
            StartsAtUtc = startsAtUtc;
            EndsAtUtc = endsAtUtc;
            CreatedBy = createdBy;
            ServiceId = serviceId;
            Notes = notes?.Trim();
            CustomerDisplayName = string.IsNullOrWhiteSpace(customerDisplayName) ? null : customerDisplayName.Trim();
            CustomerPhone = string.IsNullOrWhiteSpace(customerPhone) ? null : customerPhone.Trim();
        }

        public void AssignCustomer(Guid customerUserId)
        {
            CustomerUserId = customerUserId;
            if (Status == AppointmentStatus.Pending)
                Status = AppointmentStatus.Confirmed;
        }

        public void UpdateStatus(AppointmentStatus newStatus) => Status = newStatus;

        public void Reschedule(DateTime startsAtUtc, DateTime endsAtUtc)
        {
            if (endsAtUtc <= startsAtUtc)
                throw new ArgumentException("Horário inválido.");

            StartsAtUtc = startsAtUtc;
            EndsAtUtc = endsAtUtc;
        }
        
        public void UpdateNotes(string? notes)
            => Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }
}