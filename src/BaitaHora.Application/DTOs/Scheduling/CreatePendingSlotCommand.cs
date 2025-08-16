namespace BaitaHora.Application.DTOs.Commands.Scheduling
{
    public sealed record CreatePendingSlotCommand(
        Guid ScheduleId,
        DateTime StartsAtUtc,
        int DurationMinutes,
        Guid? ServiceId,
        string? Notes,
        string? TentativeName,
        string? TentativePhone
    );
}