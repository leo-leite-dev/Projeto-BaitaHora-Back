using BaitaHora.Domain.Entities.Commons;

namespace BaitaHora.Domain.Entities.Companies.Customers
{
    public class Customer : Base
    {
        public string PhoneE164 { get; private set; } = null!;
        public string? Name { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Customer() { }

        public Customer(string phoneE164, string? name = null)
        {
            if (string.IsNullOrWhiteSpace(phoneE164))
                throw new ArgumentException("Telefone é obrigatório.");

            PhoneE164 = phoneE164.Trim();
            Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim();
        }

        public void UpdateInfo(string? name)
        {
            if (!string.IsNullOrWhiteSpace(name)) Name = name.Trim();
        }

        public void Deactivate() => IsActive = false;
    }
}