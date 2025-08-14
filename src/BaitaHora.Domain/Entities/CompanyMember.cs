using BaitaHora.Domain.Enums;

namespace BaitaHora.Domain.Entities
{
    public class CompanyMember
    {
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; } = null!;

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public CompanyRole Role { get; private set; }
        public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;
        public bool IsActive { get; private set; } = true;

        protected CompanyMember() { }

        public CompanyMember(Guid companyId, Guid userId, CompanyRole role, DateTime joinedAt, bool isActive)
        {
            CompanyId = companyId;
            UserId = userId;
            Role = role;
            JoinedAt = joinedAt;
            IsActive = isActive;
        }
    }
}