namespace BaitaHora.Domain.Entities.Companies
{
    public class CompanyImage
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Url { get; private set; } = string.Empty;
        public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;

        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; } = null!;

        protected CompanyImage() { }

        public CompanyImage(Guid companyId, string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("A URL da imagem não pode ser vazia.");

            CompanyId = companyId;
            Url = url.Trim();
        }
    }
}