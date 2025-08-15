namespace BaitaHora.Domain.Entities.Customers
{
    public class CompanyCustomer
    {
        public Guid CompanyId { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid? PreferredProfessionalUserId { get; private set; }
        public Guid? LastServiceId { get; private set; }
        public Guid? LastProfessionalUserId { get; private set; }
        public DateTime? LastVisitAtUtc { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Company Company { get; private set; } = null!;
        public Customer Customer { get; private set; } = null!;

        private CompanyCustomer() { }

        public CompanyCustomer(Guid companyId, Guid customerId)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");
            if (customerId == Guid.Empty) throw new ArgumentException("CustomerId inválido.");

            CompanyId = companyId;
            CustomerId = customerId;
        }

        public void TouchLastVisit(Guid? serviceId, Guid professionalUserId, DateTime whenUtc)
        {
            LastServiceId = serviceId;
            LastProfessionalUserId = professionalUserId;
            LastVisitAtUtc = whenUtc;
            if (PreferredProfessionalUserId is null) PreferredProfessionalUserId = professionalUserId;
        }

        public void SetPreferredProfessional(Guid? professionalUserId) => PreferredProfessionalUserId = professionalUserId;
        public void Deactivate() => IsActive = false;
    }
}