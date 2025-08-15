namespace BaitaHora.Application.DTOs.Requests.Scheduling
{
    public sealed record AssignCustomerRequest
    {
        public Guid CustomerId { get; init; }
    }
}
