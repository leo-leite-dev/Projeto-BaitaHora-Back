using BaitaHora.Application.DTOs.Auth.Commands;
using FluentValidation;

namespace BaitaHora.Application.DTOs.Auth.Validator
{
    public sealed class RegisterOwnerWithCompanyValidator : AbstractValidator<RegisterOwnerWithCompanyCommand>
    {
        public RegisterOwnerWithCompanyValidator()
        {
            RuleFor(x => x.User.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.User.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.User.Username).NotEmpty();

            RuleFor(x => x.User.Profile.FullName).NotEmpty();
            RuleFor(x => x.User.Profile.Cpf).NotEmpty().Length(11);

            RuleFor(x => x.Company.Name).NotEmpty();
            RuleFor(x => x.Company.Address).NotNull();
        }
    }
}