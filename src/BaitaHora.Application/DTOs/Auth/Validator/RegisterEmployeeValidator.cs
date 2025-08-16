using BaitaHora.Application.DTOs.Auth.Commands;
using FluentValidation;

namespace BaitaHora.Application.DTOs.Auth.Validator
{
    public sealed class RegisterEmployeeValidator : AbstractValidator<RegisterEmployeeCommand>
    {
        public RegisterEmployeeValidator()
        {
            RuleFor(x => x.ActorUserId).NotEmpty();
            RuleFor(x => x.CompanyId).NotEmpty();
            RuleFor(x => x.User.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.User.Password).NotEmpty().MinimumLength(8);
            RuleFor(x => x.User.Username).NotEmpty();
            RuleFor(x => x.User.Profile.FullName).NotEmpty();
            RuleFor(x => x.Role).NotEmpty();
        }
    }
}