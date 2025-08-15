using BaitaHora.Domain.Entities.Commons;

namespace BaitaHora.Domain.Entities
{
    public class CompanyCustomerProfessional : Base
    {
        public Guid CompanyId { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid ProfessionalUserId { get; private set; }
        public bool IsPrimary { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Company Company { get; private set; } = null!;
        public Customer Customer { get; private set; } = null!;
        public User ProfessionalUser { get; private set; } = null!;

        private CompanyCustomerProfessional() { }

        public CompanyCustomerProfessional(Guid companyId, Guid customerId, Guid professionalUserId, bool isPrimary = true)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");
            if (customerId == Guid.Empty) throw new ArgumentException("CustomerId inválido.");
            if (professionalUserId == Guid.Empty) throw new ArgumentException("ProfessionalUserId inválido.");
            if (customerId == professionalUserId) throw new ArgumentException("Cliente e profissional não podem ser a mesma pessoa.");

            CompanyId = companyId;
            CustomerId = customerId;
            ProfessionalUserId = professionalUserId;
            IsPrimary = isPrimary;
        }

        public void SetPrimary(bool isPrimary) => IsPrimary = isPrimary;
        public void Deactivate() => IsActive = false;
    }
}