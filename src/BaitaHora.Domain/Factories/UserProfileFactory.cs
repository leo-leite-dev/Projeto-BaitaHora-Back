using BaitaHora.Domain.Entities;
using BaitaHora.Domain.Entities.Users;

namespace BaitaHora.Domain.Factories
{
    public static class UserProfileFactory
    {
        public static UserProfile Create(
            string fullName,
            string cpf,
            Address address,
            string? rg = null,
            DateTime? birthDate = null,
            string? phone = null,
            DateTime? admissionDate = null,
            string? profileImageUrl = null)
        {
            return new UserProfile(
                fullName: fullName,
                cpf: cpf,
                address: address,
                rg: rg,
                birthDate: birthDate,
                phone: phone,
                admissionDate: admissionDate,
                profileImageUrl: profileImageUrl
            );
        }
    }
}