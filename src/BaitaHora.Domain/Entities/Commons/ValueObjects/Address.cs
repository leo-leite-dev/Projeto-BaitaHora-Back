using BaitaHora.Domain.Exceptions;

namespace BaitaHora.Domain.Entities.Commons.ValueObjects
{
    public class Address
    {
        public string Street { get; private set; } = string.Empty;
        public string Number { get; private set; } = string.Empty;
        public string Complement { get; private set; } = string.Empty;
        public string Neighborhood { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
        public string ZipCode { get; private set; } = string.Empty;

        private Address() { }

        private Address(string street, string number, string neighborhood, string city,
                        string state, string zipCode, string? complement)
        {
            Street = street;
            Number = number;
            Neighborhood = neighborhood;
            City = city;
            State = state;
            ZipCode = zipCode;
            Complement = complement ?? string.Empty;
        }

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

            var streetN = TrimOrEmpty(street);
            var numberN = TrimOrEmpty(number);
            var neighborhoodN = TrimOrEmpty(neighborhood);
            var cityN = TrimOrEmpty(city);
            var stateN = TrimOrEmpty(state).ToUpperInvariant();
            var zipCodeN = TrimOrEmpty(zipCode);
            var complementN = TrimOrNull(complement);

            if (streetN.Length < 3 || streetN.Length > 200)
                throw new UserException("Rua deve conter entre 3 e 200 caracteres.");

            if (string.IsNullOrWhiteSpace(numberN) || numberN.Length > 20)
                throw new UserException("Número é obrigatório e deve ter no máximo 20 caracteres.");

            if (string.IsNullOrWhiteSpace(neighborhoodN) || neighborhoodN.Length > 100)
                throw new UserException("Bairro é obrigatório e deve ter no máximo 100 caracteres.");

            if (string.IsNullOrWhiteSpace(cityN) || cityN.Length > 100)
                throw new UserException("Cidade é obrigatória e deve ter no máximo 100 caracteres.");

            if (stateN.Length != 2)
                throw new UserException("Estado deve conter exatamente 2 letras (UF).");

            if (zipCodeN.Length < 8 || zipCodeN.Length > 9)
                throw new UserException("CEP inválido.");

            return new Address(
                street: streetN,
                number: numberN,
                neighborhood: neighborhoodN,
                city: cityN,
                state: stateN,
                zipCode: zipCodeN,
                complement: complementN
            );
        }
    }
}