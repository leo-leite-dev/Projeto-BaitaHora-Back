namespace BaitaHora.Application.DTOs.Requests.Scheduling
{
    public sealed record CreateAppointmentRequest
    {
        public Guid ScheduleId { get; init; }
        public DateTime StartsAtUtc { get; init; }
        public DateTime EndsAtUtc { get; init; }
        public Guid? ServiceId { get; init; }
        public string? Notes { get; init; }
        public Guid? CustomerUserId { get; init; }
        public string? CustomerDisplayName { get; init; }
        public string? CustomerPhone { get; init; }
        public string CreatedBy { get; init; } = "Staff";
    }
}