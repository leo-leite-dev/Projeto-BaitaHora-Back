using BaitaHora.Domain.Entities.Users;

namespace BaitaHora.Domain.Entities
{
    public class Company
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; } = string.Empty;
        public string? Document { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Address Address { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public ICollection<CompanyMember> Members { get; private set; } = new List<CompanyMember>();
        public CompanyImage? Image { get; private set; }

        protected Company() { }
        public Company(string name, Address address, string? document = null)
        {
            Name = name.Trim();
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Document = string.IsNullOrWhiteSpace(document) ? null : document.Trim();
        }
    }
}