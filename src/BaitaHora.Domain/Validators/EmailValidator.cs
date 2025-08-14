using System.Text.RegularExpressions;
using BaitaHora.Domain.Exceptions;

namespace BaitaHora.Domain.Validators
{
    public static class EmailValidator
    {
        private static readonly Regex _regex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string EnsureValid(string email)
        {
            email = email.Trim().ToLower();

            if (!_regex.IsMatch(email))
                throw new UserException("Formato de e-mail inválido.");

            if (email.Length > 254)
                throw new UserException("E-mail muito longo.");

            return email;
        }
    }
}