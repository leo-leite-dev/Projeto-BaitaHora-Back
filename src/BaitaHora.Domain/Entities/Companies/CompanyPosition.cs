using BaitaHora.Domain.Entities.Commons;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Domain.Entities.Companies
{
    public class CompanyPosition: Base
    {
        public Guid CompanyId { get; private set; }
        public string Name { get; private set; } = null!;
        public CompanyRole AccessLevel { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Company Company { get; private set; } = null!;

        private CompanyPosition() { }

        public CompanyPosition(Guid companyId, string name, CompanyRole accessLevel)
        {
            if (companyId == Guid.Empty) throw new ArgumentException("CompanyId inválido.");
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome do cargo é obrigatório.");
            CompanyId = companyId;
            Name = name.Trim();
            AccessLevel = accessLevel;
        }


        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome do cargo é obrigatório.");
            Name = name.Trim();
        }

        public void SetAccessLevel(CompanyRole level) => AccessLevel = level;
        public void Deactivate() => IsActive = false;
    }
}