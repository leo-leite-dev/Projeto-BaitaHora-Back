using BaitaHora.Application.DTOs.Requests.Scheduling;
using FluentValidation;

namespace BaitaHora.Application.Validators.Scheduling
{
    public sealed class RescheduleAppointmentRequestValidator : AbstractValidator<RescheduleAppointmentRequest>
    {
        public RescheduleAppointmentRequestValidator()
        {
            RuleFor(x => x.StartsAtUtc)
                .NotEmpty().WithMessage("StartsAtUtc é obrigatório.");

            RuleFor(x => x.EndsAtUtc)
                .NotEmpty().WithMessage("EndsAtUtc é obrigatório.")
                .Must((req, ends) => ends > req.StartsAtUtc)
                .WithMessage("EndsAtUtc deve ser maior que StartsAtUtc.");
        }
    }
}