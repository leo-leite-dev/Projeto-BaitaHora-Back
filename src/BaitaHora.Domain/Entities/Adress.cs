using BaitaHora.Domain.Exceptions;

namespace BaitaHora.Domain.Entities.Users
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

        public Address(string street, string number, string neighborhood, string city,
                       string state, string zipCode, string? complement = null)
        {
            if (string.IsNullOrWhiteSpace(street) || street.Length < 3 || street.Length > 200)
                throw new UserException("Rua deve conter entre 3 e 200 caracteres.");

            if (string.IsNullOrWhiteSpace(number) || number.Length > 20)
                throw new UserException("Número é obrigatório e deve ter no máximo 20 caracteres.");

            if (string.IsNullOrWhiteSpace(neighborhood) || neighborhood.Length > 100)
                throw new UserException("Bairro é obrigatório e deve ter no máximo 100 caracteres.");

            if (string.IsNullOrWhiteSpace(city) || city.Length > 100)
                throw new UserException("Cidade é obrigatória e deve ter no máximo 100 caracteres.");

            if (string.IsNullOrWhiteSpace(state) || state.Length != 2)
                throw new UserException("Estado deve conter exatamente 2 letras (UF).");

            if (string.IsNullOrWhiteSpace(zipCode) || zipCode.Length < 8 || zipCode.Length > 9)
                throw new UserException("CEP inválido.");

            Street = street.Trim();
            Number = number.Trim();
            Complement = complement?.Trim() ?? string.Empty;
            Neighborhood = neighborhood.Trim();
            City = city.Trim();
            State = state.Trim().ToUpper();
            ZipCode = zipCode.Trim();
        }
    }
}