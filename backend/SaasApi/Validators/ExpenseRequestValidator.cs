using FluentValidation;
using SaasApi.Models;

namespace SaasApi.Validators;

public class ExpenseRequestValidator : AbstractValidator<ExpenseRequest>
{
    public ExpenseRequestValidator()
    {
        RuleFor(x => x.Texto)
            .NotEmpty().WithMessage("O texto da despesa é obrigatório.")
            .MinimumLength(5).WithMessage("O texto deve ter pelo menos 5 caracteres para a IA entender.")
            .MaximumLength(500).WithMessage("Calma! Texto muito longo (máximo 500 caracteres).");
    }
}