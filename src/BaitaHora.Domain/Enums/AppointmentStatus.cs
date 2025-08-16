namespace BaitaHora.Domain.Enums
{
    public enum AppointmentStatus
    {
        Pending = 0,
        Confirmed = 1,
        Cancelled = 2,
        NoShow = 3,
        Completed = 4
    }

    public enum AppointmentCreatedBy
    {
        Staff = 0,
        Chatbot = 1,
        Customer = 2
    }
}