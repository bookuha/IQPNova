using FluentValidation;

namespace IQP.Application.Usecases.AlgoTasks.Translate;

public class TranslateAlgoTaskCommandValidator : AbstractValidator<TranslateAlgoTaskCommand>
{
    public TranslateAlgoTaskCommandValidator()
    {
        RuleFor(c => c.AlgoTaskId).NotEmpty();
        RuleFor(c => c.InitialCodeSnippet).NotNull().SetValidator(new CodeSnippetValidator());
    }
}