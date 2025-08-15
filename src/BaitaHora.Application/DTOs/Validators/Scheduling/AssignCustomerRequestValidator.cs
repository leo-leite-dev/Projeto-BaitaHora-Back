using BaitaHora.Application.DTOs.Requests.Scheduling;
using FluentValidation;

namespace BaitaHora.Application.Validators.Scheduling
{
    public sealed class AssignCustomerRequestValidator : AbstractValidator<AssignCustomerRequest>
    {
        public AssignCustomerRequestValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId é obrigatório.");
        }
    }
}