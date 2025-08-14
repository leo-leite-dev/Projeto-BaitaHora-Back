using BaitaHora.Domain.Entities.Commons;
using BaitaHora.Domain.Entities.Users;
using BaitaHora.Domain.Exceptions;
using BaitaHora.Domain.Validators;

namespace BaitaHora.Domain.Entities
{
    public class UserProfile : Base
    {
        public string FullName { get; private set; } = string.Empty;
        public string CPF { get; private set; } = string.Empty;
        public string? RG { get; private set; }
        public DateTime? BirthDate { get; private set; }
        public string? Phone { get; private set; }
        public Address Address { get; private set; } = default!;
        public string? ProfileImageUrl { get; private set; }

        private UserProfile() { }

        public UserProfile(
            string fullName,
            string cpf,
            Address address,
            string? rg = null,
            DateTime? birthDate = null,
            string? phone = null,
            DateTime? admissionDate = null,
            string? profileImageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 3 || fullName.Length > 200)
                throw new UserException("O nome completo deve ter entre 3 e 200 caracteres.");

            cpf = CPFValidator.EnsureValid(cpf);

            if (!string.IsNullOrWhiteSpace(rg))
            {
                rg = rg.Trim();
                if (rg.Length < 5 || rg.Length > 20)
                    throw new UserException("RG deve ter entre 5 e 20 caracteres.");
                RG = rg;
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                PhoneValidator.Validate(phone);
                Phone = phone.Trim();
            }

            if (birthDate.HasValue)
            {
                var birth = birthDate.Value.Date;

                if (birth > DateTime.UtcNow.Date)
                    throw new UserException("A data de nascimento não pode estar no futuro.");

                if (birth < DateTime.UtcNow.AddYears(-100))
                    throw new UserException("Data de nascimento muito antiga.");

                var age = DateTime.UtcNow.Year - birth.Year;
                if (birth > DateTime.UtcNow.AddYears(-age)) age--;

                if (age < 18)
                    throw new UserException("Usuário deve ter pelo menos 18 anos.");

                BirthDate = birth;
            }

            if (admissionDate.HasValue && admissionDate.Value.Date > DateTime.UtcNow.Date)
                throw new UserException("A data de admissão não pode estar no futuro.");

            FullName = fullName.Trim();
            CPF = cpf;
            ProfileImageUrl = profileImageUrl?.Trim();
            Address = address ?? throw new UserException("Endereço é obrigatório.");
        }
    }
}