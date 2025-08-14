namespace BaitaHora.Application.DTOs.Requests.Scheduling
{
    public sealed record CreateServiceCatalogItemRequest
    {
        public string Name { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public decimal? Price { get; init; }
    }
}