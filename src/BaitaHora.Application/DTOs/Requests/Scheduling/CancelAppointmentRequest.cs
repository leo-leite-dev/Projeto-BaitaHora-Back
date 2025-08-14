namespace BaitaHora.Application.DTOs.Requests.Scheduling
{
    public sealed record CancelAppointmentRequest
    {
        public string? Reason { get; init; }
    }
}