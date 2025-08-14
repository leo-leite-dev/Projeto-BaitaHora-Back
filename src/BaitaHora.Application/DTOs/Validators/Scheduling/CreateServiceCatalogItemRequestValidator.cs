using BaitaHora.Application.DTOs.Requests.Scheduling;
using FluentValidation;

namespace BaitaHora.Application.Validators.Scheduling
{
    public sealed class CreateServiceCatalogItemRequestValidator : AbstractValidator<CreateServiceCatalogItemRequest>
    {
        public CreateServiceCatalogItemRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name é obrigatório.")
                .MaximumLength(120);

            RuleFor(x => x.DurationMinutes)
                .InclusiveBetween(1, 24 * 60)
                .WithMessage("DurationMinutes deve estar entre 1 e 1440.");

            RuleFor(x => x.Price)
                .Must(p => p is null || p >= 0)
                .WithMessage("Price não pode ser negativo.");
        }
    }
}