namespace BaitaHora.Application.DTOs.Responses.Scheduling
{
    public sealed record ScheduleResponse
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public Guid CompanyId { get; init; }
        public bool IsActive { get; init; }
        public DateTimeOffset CreatedAtUtc { get; init; }
    }
}
