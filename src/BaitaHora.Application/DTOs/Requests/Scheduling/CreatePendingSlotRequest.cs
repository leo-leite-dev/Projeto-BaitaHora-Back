namespace BaitaHora.Application.DTOs.Requests.Scheduling
{
    public sealed record CreatePendingSlotRequest
    {
        public DateTime StartsAtUtc { get; init; }
        public int DurationMinutes { get; init; }
        public string? TentativeName { get; init; }
        public string? TentativePhone { get; init; }
        public Guid? ServiceId { get; init; }
        public string? Notes { get; init; }
    }
}