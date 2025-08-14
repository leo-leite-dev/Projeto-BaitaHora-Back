using BaitaHora.Application.DTOs.Requests.Scheduling;
using FluentValidation;

namespace BaitaHora.Application.Validators.Scheduling
{
    public sealed class AssignCustomerRequestValidator : AbstractValidator<AssignCustomerRequest>
    {
        public AssignCustomerRequestValidator()
        {
            RuleFor(x => x.CustomerUserId)
                .NotEmpty().WithMessage("CustomerUserId é obrigatório.");
        }
    }
}