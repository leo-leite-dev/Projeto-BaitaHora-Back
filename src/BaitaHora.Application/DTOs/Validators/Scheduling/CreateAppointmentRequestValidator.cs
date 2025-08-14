using BaitaHora.Application.DTOs.Requests.Scheduling;
using FluentValidation;

namespace BaitaHora.Application.Validators.Scheduling
{
    public sealed class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
    {
        public CreateAppointmentRequestValidator()
        {
            RuleFor(x => x.ScheduleId)
                .NotEmpty().WithMessage("ScheduleId é obrigatório.");

            RuleFor(x => x.StartsAtUtc)
                .NotEmpty().WithMessage("StartsAtUtc é obrigatório.");

            RuleFor(x => x.EndsAtUtc)
                .NotEmpty().WithMessage("EndsAtUtc é obrigatório.")
                .Must((req, ends) => ends > req.StartsAtUtc)
                .WithMessage("EndsAtUtc deve ser maior que StartsAtUtc.");

            RuleFor(x => x.CustomerDisplayName)
                .MaximumLength(120);

            RuleFor(x => x.CustomerPhone)
                .MaximumLength(40);

            RuleFor(x => x.Notes)
                .MaximumLength(1024);

            RuleFor(x => x.CreatedBy)
                .NotEmpty()
                .Must(v =>
                    v.Equals("Staff", StringComparison.OrdinalIgnoreCase) ||
                    v.Equals("Chatbot", StringComparison.OrdinalIgnoreCase) ||
                    v.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                .WithMessage("CreatedBy deve ser Staff, Chatbot ou Customer.");
        }
    }
}