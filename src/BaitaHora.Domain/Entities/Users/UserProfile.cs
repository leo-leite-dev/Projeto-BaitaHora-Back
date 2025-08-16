using BaitaHora.Domain.Entities.Commons;
using BaitaHora.Domain.Entities.Commons.ValueObjects;
using BaitaHora.Domain.Exceptions;
using BaitaHora.Domain.Validators;

namespace BaitaHora.Domain.Entities.Users
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

        private UserProfile(
            string fullName,
            string cpf,
            Address address,
            string? rg,
            DateTime? birthDate,
            string? phone,
            string? profileImageUrl)
        {
            FullName = fullName;
            CPF = cpf;
            Address = address;
            RG = rg;
            BirthDate = birthDate;
            Phone = phone;
            ProfileImageUrl = profileImageUrl;
        }

        public static UserProfile Create(
            string fullName,
            string cpf,
            Address address,
            string? rg = null,
            DateTime? birthDate = null,
            string? phone = null,
            string? profileImageUrl = null)
        {
            fullName = (fullName ?? string.Empty).Trim();
            rg = string.IsNullOrWhiteSpace(rg) ? null : rg.Trim();
            phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
            profileImageUrl = string.IsNullOrWhiteSpace(profileImageUrl) ? null : profileImageUrl.Trim();

            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 3 || fullName.Length > 200)
                throw new UserException("O nome completo deve ter entre 3 e 200 caracteres.");

            var cpfValid = CPFValidator.EnsureValid(cpf);

            if (!string.IsNullOrWhiteSpace(rg) && (rg.Length < 5 || rg.Length > 20))
                throw new UserException("RG deve ter entre 5 e 20 caracteres.");

            if (!string.IsNullOrWhiteSpace(phone))
                PhoneValidator.Validate(phone);

            if (birthDate.HasValue)
            {
                var birth = birthDate.Value.Date;
                if (birth > DateTime.UtcNow.Date) throw new UserException("A data de nascimento não pode estar no futuro.");
                if (birth < DateTime.UtcNow.AddYears(-100)) throw new UserException("Data de nascimento muito antiga.");

                var age = DateTime.UtcNow.Year - birth.Year;
                if (birth > DateTime.UtcNow.AddYears(-age)) age--;
                if (age < 18) throw new UserException("Usuário deve ter pelo menos 18 anos.");
                birthDate = birth;
            }

            if (address is null) throw new UserException("Endereço é obrigatório.");

            return new UserProfile(
                fullName: fullName,
                cpf: cpfValid,
                address: address,
                rg: rg,
                birthDate: birthDate,
                phone: phone,
                profileImageUrl: profileImageUrl
            );
        }

        public void UpdateFullName(string fullName)
        {
            fullName = (fullName ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 3 || fullName.Length > 200)
                throw new UserException("O nome completo deve ter entre 3 e 200 caracteres.");
            FullName = fullName;
            Touch();
        }

        public void UpdateBirthDate(DateTime? birthDate)
        {
            if (!birthDate.HasValue)
            {
                BirthDate = null;
                Touch();
                return;
            }

            var birth = birthDate.Value.Date;
            if (birth > DateTime.UtcNow.Date) throw new UserException("A data de nascimento não pode estar no futuro.");
            if (birth < DateTime.UtcNow.AddYears(-100)) throw new UserException("Data de nascimento muito antiga.");

            var age = DateTime.UtcNow.Year - birth.Year;
            if (birth > DateTime.UtcNow.AddYears(-age)) age--;
            if (age < 18) throw new UserException("Usuário deve ter pelo menos 18 anos.");

            BirthDate = birth;
            Touch();
        }

        public void UpdatePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                Phone = null;
                Touch();
                return;
            }
            PhoneValidator.Validate(phone);
            Phone = phone.Trim();
            Touch();
        }

        public void UpdateAddress(Address address)
        {
            if (address is null) throw new UserException("Endereço é obrigatório.");
            Address = address;
            Touch();
        }

        public void SetProfileImage(string? url)
        {
            ProfileImageUrl = string.IsNullOrWhiteSpace(url) ? null : url.Trim();
            Touch();
        }
    }
}