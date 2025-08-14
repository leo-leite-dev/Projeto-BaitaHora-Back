using BaitaHora.Application.DTOs.Requests.Scheduling;
using FluentValidation;

namespace BaitaHora.Application.Validators.Scheduling
{
    public sealed class UpdateAppointmentStatusRequestValidator : AbstractValidator<UpdateAppointmentStatusRequest>
    {
        private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
        { "Pending", "Confirmed", "Cancelled", "NoShow" };

        public UpdateAppointmentStatusRequestValidator()
        {
            RuleFor(x => x.NewStatus)
                .NotEmpty().WithMessage("NewStatus é obrigatório.")
                .Must(s => Allowed.Contains(s))
                .WithMessage("NewStatus deve ser Pending, Confirmed, Cancelled ou NoShow.");
        }
    }
}