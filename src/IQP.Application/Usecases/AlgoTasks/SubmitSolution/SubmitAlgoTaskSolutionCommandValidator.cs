using FluentValidation;

namespace IQP.Application.Usecases.AlgoTasks.SubmitSolution;

public class SubmitAlgoTaskSolutionCommandValidator : AbstractValidator<SubmitAlgoTaskSolutionCommand>
{
    public SubmitAlgoTaskSolutionCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.AlgoTaskId).NotEmpty();
        RuleFor(c => c.LanguageId).NotEmpty();
    }
}