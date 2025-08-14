using BaitaHora.Domain.Exceptions;

namespace BaitaHora.Domain.Validators
{
    public static class CPFValidator
    {
        public static string EnsureValid(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                throw new UserException("CPF é obrigatório.");

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");

            if (cpf.Length != 11 || !cpf.All(char.IsDigit))
                throw new UserException("CPF inválido.");

            if (cpf.Distinct().Count() == 1)
                throw new UserException("CPF inválido.");

            var cpfArray = cpf.Select(c => int.Parse(c.ToString())).ToArray();

            var sum = 0;
            for (int i = 0; i < 9; i++)
                sum += cpfArray[i] * (10 - i);

            var firstDigit = sum % 11 < 2 ? 0 : 11 - (sum % 11);
            if (cpfArray[9] != firstDigit)
                throw new UserException("CPF inválido.");

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += cpfArray[i] * (11 - i);

            var secondDigit = sum % 11 < 2 ? 0 : 11 - (sum % 11);
            if (cpfArray[10] != secondDigit)
                throw new UserException("CPF inválido.");

            return cpf;
        }
    }
}