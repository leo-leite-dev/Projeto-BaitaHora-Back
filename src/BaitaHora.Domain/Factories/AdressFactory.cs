using BaitaHora.Domain.Entities.Users;

namespace BaitaHora.Domain.Factories
{
    public static class AddressFactory
    {
        public static Address Create(
            string street,
            string number,
            string neighborhood,
            string city,
            string state,
            string zipCode,
            string? complement = null)
        {
            return new Address(
                street: street,
                number: number,
                neighborhood: neighborhood,
                city: city,
                state: state,
                zipCode: zipCode,
                complement: complement
            );
        }
    }
}