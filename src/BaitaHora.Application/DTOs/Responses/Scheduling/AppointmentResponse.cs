namespace BaitaHora.Application.DTOs.Responses.Scheduling
{
    public sealed record AppointmentResponse
    {
        public Guid Id { get; init; }
        public Guid ScheduleId { get; init; }

        public DateTimeOffset StartsAtUtc { get; init; }
        public DateTimeOffset EndsAtUtc { get; init; }

        public string Status { get; init; } = string.Empty;
        public string CreatedBy { get; init; } = string.Empty;

        public Guid? ServiceId { get; init; }
        public Guid? CustomerUserId { get; init; }
        public string? CustomerDisplayName { get; init; }
        public string? CustomerPhone { get; init; }
        public string? Notes { get; init; }
    }
}