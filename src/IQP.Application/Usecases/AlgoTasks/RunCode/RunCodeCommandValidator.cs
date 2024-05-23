using FluentValidation;

namespace IQP.Application.Usecases.AlgoTasks.RunCode;

public class RunCodeCommandValidator : AbstractValidator<RunCodeCommand>
{
    public RunCodeCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.Tests).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.LanguageId).NotEmpty();
    }
}