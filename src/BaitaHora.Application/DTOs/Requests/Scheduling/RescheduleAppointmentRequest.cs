namespace BaitaHora.Application.DTOs.Requests.Scheduling
{
    public sealed record RescheduleAppointmentRequest
    {
        public DateTime StartsAtUtc { get; init; }

        public DateTime EndsAtUtc { get; init; }
    }
}