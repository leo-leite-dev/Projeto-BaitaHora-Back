// Application/Validators/Chatbot/CreateCustomerQuickRequestValidator.cs
using BaitaHora.Application.DTOs.Chatbot.Requests;
using FluentValidation;

public sealed class CreateCustomerQuickRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerQuickRequestValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.PhoneE164)
            .NotEmpty()
            .Matches(@"^\+[1-9]\d{1,14}$") 
            .WithMessage("PhoneE164 deve estar no formato E.164 (ex.: +5511999990000).");
        RuleFor(x => x.RoleName).MaximumLength(80).When(x => !string.IsNullOrWhiteSpace(x.RoleName));
    }
}