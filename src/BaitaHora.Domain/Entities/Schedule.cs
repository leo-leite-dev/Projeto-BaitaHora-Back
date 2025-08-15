using BaitaHora.Domain.Entities.Commons;

namespace BaitaHora.Domain.Entities
{
    public class Schedule : Base
    {
        public Guid UserId { get; private set; }
        public Guid CompanyId { get; private set; }
        public bool IsActive { get; private set; } = true;

        public User User { get; private set; } = null!;
        public Company Company { get; private set; } = null!;
        public ICollection<Appointment> Appointments { get; private set; } = new List<Appointment>();

        private Schedule() { }

        public Schedule(Guid userId, Guid companyId)
        {
            if (userId == Guid.Empty) throw new ArgumentException("UserId inválido.");
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");

            UserId = userId;
            CompanyId = companyId;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }

    public enum AppointmentStatus { Pending = 0, Confirmed = 1, Cancelled = 2, NoShow = 3 }
    public enum AppointmentCreatedBy { Staff = 0, Chatbot = 1, Customer = 2 }
}