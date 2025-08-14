using System.Text.RegularExpressions;
using BaitaHora.Domain.Exceptions;

namespace BaitaHora.Domain.Validators
{
    public static class PhoneValidator
    {
        private static readonly Regex _phoneRegex = new Regex(
            @"^\+?(\d{1,3})?\s*(\(?\d{2}\)?\s*)?\d{4,5}-?\d{4}$",
            RegexOptions.Compiled
        );

        public static void Validate(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new UserException("O telefone não pode estar em branco.");

            var trimmedPhone = phone.Trim();

            if (!_phoneRegex.IsMatch(trimmedPhone))
                throw new UserException("Formato de telefone inválido.");
        }
    }
}
