namespace BaitaHora.Application.DTOs.Responses.Scheduling
{
    public sealed record ServiceCatalogItemResponse
    {
        public Guid Id { get; init; }
        public Guid CompanyId { get; init; }

        public string Name { get; init; } = string.Empty;
        public int DurationMinutes { get; init; }
        public decimal? Price { get; init; }

        public bool IsActive { get; init; }
    }
}