namespace BaitaHora.Domain.Entities
{
    public class ServiceCatalogItem
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid CompanyId { get; private set; }
        public string Name { get; private set; }
        public int DurationMinutes { get; private set; }
        public decimal? Price { get; private set; }
        public bool IsActive { get; private set; } = true;

        private ServiceCatalogItem() { }
        public ServiceCatalogItem(Guid companyId, string name, int durationMinutes, decimal? price = null)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome do serviço é obrigatório.");
            if (durationMinutes <= 0) throw new ArgumentException("Duração inválida.");

            CompanyId = companyId;
            Name = name.Trim();
            DurationMinutes = durationMinutes;
            Price = price;
        }

        public void Deactivate() => IsActive = false;
    }
}