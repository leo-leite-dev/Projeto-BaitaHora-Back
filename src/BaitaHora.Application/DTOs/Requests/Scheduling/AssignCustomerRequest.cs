namespace BaitaHora.Application.DTOs.Requests.Scheduling
{
    public sealed record AssignCustomerRequest
    {
        public Guid CustomerUserId { get; init; }
    }
}
