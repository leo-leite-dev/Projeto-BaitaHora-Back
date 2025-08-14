namespace BaitaHora.Application.DTOs.Requests.Scheduling
{
    public sealed record UpdateAppointmentStatusRequest
    {
        public string NewStatus { get; init; } = "Pending";
    }
}