using FluentValidation;

namespace IQP.Application.Usecases.AlgoTasks.TrySolution;

public class TryAlgoTaskSolutionCommandValidator : AbstractValidator<TryAlgoTaskSolutionCommand>
{
    public TryAlgoTaskSolutionCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.AlgoTaskId).NotEmpty();
        RuleFor(c => c.LanguageId).NotEmpty();
    }
}