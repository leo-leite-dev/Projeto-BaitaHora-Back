using BaitaHora.Application.DTOs.Auth.Commands;
using FluentValidation;

namespace BaitaHora.Application.DTOs.Auth.Validator
{
    public sealed class AuthenticateValidator : AbstractValidator<AuthenticateCommand>
    {
        public AuthenticateValidator()
        {
            RuleFor(x => x.UsernameOrEmail).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}