using BaitaHora.Domain.Entities.Users;

namespace BaitaHora.Domain.Factories
{
    public static class AddressFactory
    {
        public static Address Create(
            string? street,
            string? number,
            string? neighborhood,
            string? city,
            string? state,
            string? zipCode,
            string? complement = null)
        {
            static string TrimOrEmpty(string? s) => (s ?? string.Empty).Trim();
            static string? TrimOrNull(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

            return new Address(
                street: TrimOrEmpty(street),
                number: TrimOrEmpty(number),
                neighborhood: TrimOrEmpty(neighborhood),
                city: TrimOrEmpty(city),
                state: TrimOrEmpty(state),
                zipCode: TrimOrEmpty(zipCode),
                complement: TrimOrNull(complement)
            );
        }
    }
}