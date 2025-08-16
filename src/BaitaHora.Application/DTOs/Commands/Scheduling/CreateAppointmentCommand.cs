using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.DTOs.Commands.Scheduling
{
    public sealed record CreateAppointmentCommand(
        Guid ScheduleId,
        DateTime StartsAtUtc,
        DateTime EndsAtUtc,
        AppointmentCreatedBy CreatedBy,
        Guid? ServiceId,
        string? Notes,
        string? CustomerDisplayName,
        string? CustomerPhone,
        Guid? CustomerId
    );
}