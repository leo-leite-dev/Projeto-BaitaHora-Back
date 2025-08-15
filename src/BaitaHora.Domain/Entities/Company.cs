using BaitaHora.Domain.Entities.Commons;
using BaitaHora.Domain.Entities.Users;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Domain.Entities
{
    public class Company : Base
    {
        public string Name { get; private set; } = string.Empty;
        public string? Document { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Address Address { get; private set; } = null!;

        public ICollection<CompanyMember> Members { get; private set; } = new List<CompanyMember>();
        public ICollection<CompanyPosition> Positions { get; private set; } = new List<CompanyPosition>(); // << novo

        public CompanyImage? Image { get; private set; }

        protected Company() { }

        public Company(string name, Address address, string? document = null)
        {
            UpdateName(name);
            UpdateAddress(address);
            UpdateDocument(document);
        }

        public static Company Create(string name, Address address, string? document = null)
        {
            if (address is null) throw new ArgumentNullException(nameof(address));

            var c = new Company();
            c.ApplyName(name);
            c.ApplyAddress(address);
            c.ApplyDocument(document);
            return c;
        }

        public void UpdateName(string? name) => ApplyName(name);
        public void UpdateDocument(string? document) => ApplyDocument(document);
        public void UpdateAddress(Address address) => ApplyAddress(address);
        public void SetImage(CompanyImage? image) => Image = image;

        private void ApplyName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Nome da empresa é obrigatório.", nameof(name));

            var trimmed = name.Trim();
            if (trimmed.Length < 2)
                throw new ArgumentException("Nome da empresa é muito curto.", nameof(name));

            Name = trimmed;
        }

        private void ApplyDocument(string? document)
        {
            Document = string.IsNullOrWhiteSpace(document) ? null : document.Trim();
        }

        private void ApplyAddress(Address address)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
        }

        public CompanyPosition CreatePosition(string name, CompanyRole level)
        {
            var normalized = (name ?? string.Empty).Trim();
            if (normalized.Length == 0)
                throw new ArgumentException("Nome obrigatório.", nameof(name));

            if (Positions.Any(p => string.Equals(p.Name, normalized, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Já existe cargo '{normalized}' nesta empresa.");

            var pos = new CompanyPosition(Id, normalized, level);
            Positions.Add(pos);
            return pos;
        }

        public void RenamePosition(Guid positionId, string newName)
        {
            var pos = Positions.FirstOrDefault(p => p.Id == positionId)
                      ?? throw new KeyNotFoundException("Cargo não encontrado.");

            var normalized = (newName ?? string.Empty).Trim();
            if (normalized.Length == 0)
                throw new ArgumentException("Nome obrigatório.", nameof(newName));

            if (Positions.Any(p => p.Id != positionId &&
                                   string.Equals(p.Name, normalized, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Já existe cargo '{normalized}' nesta empresa.");

            pos.Rename(normalized);
        }

        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}