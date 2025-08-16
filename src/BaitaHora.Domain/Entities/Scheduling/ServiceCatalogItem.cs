using BaitaHora.Domain.Entities.Commons;

namespace BaitaHora.Domain.Entities.Scheduling
{
    public class ServiceCatalogItem : Base
    {
        public Guid CompanyId { get; private set; }
        public string Name { get; private set; }
        public int DurationMinutes { get; private set; }
        public decimal? Price { get; private set; }
        public bool IsActive { get; private set; } = true;

        private ServiceCatalogItem() { }

        public ServiceCatalogItem(Guid companyId, string name, int durationMinutes, decimal? price = null)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.", nameof(companyId));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome do serviço é obrigatório.", nameof(name));
            if (durationMinutes <= 0) throw new ArgumentException("Duração inválida.", nameof(durationMinutes));
            if (price.HasValue && price.Value < 0) throw new ArgumentException("Preço não pode ser negativo.", nameof(price));

            CompanyId = companyId;
            Name = name.Trim();
            DurationMinutes = durationMinutes;
            Price = price;
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome do serviço é obrigatório.", nameof(name));
            Name = name.Trim();
        }

        public void ChangeDuration(int durationMinutes)
        {
            if (durationMinutes <= 0) throw new ArgumentException("Duração inválida.", nameof(durationMinutes));
            DurationMinutes = durationMinutes;
        }

        public void ChangePrice(decimal? price)
        {
            if (price.HasValue && price.Value < 0) throw new ArgumentException("Preço não pode ser negativo.", nameof(price));
            Price = price;
        }

        public void Deactivate() => IsActive = false;

        public void Activate() => IsActive = true;
    }
}