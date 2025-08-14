using BaitaHora.Application.DTOs.Requests.Scheduling;
using FluentValidation;

namespace BaitaHora.Application.Validators.Scheduling
{
    public sealed class CreatePendingSlotRequestValidator : AbstractValidator<CreatePendingSlotRequest>
    {
        public CreatePendingSlotRequestValidator()
        {
            RuleFor(x => x.StartsAtUtc)
                .NotEmpty().WithMessage("StartsAtUtc é obrigatório.");

            RuleFor(x => x.DurationMinutes)
                .InclusiveBetween(1, 24 * 60)
                .WithMessage("DurationMinutes deve estar entre 1 e 1440 minutos.");

            RuleFor(x => x.TentativeName)
                .MaximumLength(120);

            RuleFor(x => x.TentativePhone)
                .MaximumLength(40);

            RuleFor(x => x.Notes)
                .MaximumLength(1024);
        }
    }
}