using FluentValidation;

namespace IQP.Application.Usecases.AlgoTasks.Create;

public class CreateAlgoTaskCommandValidator : AbstractValidator<CreateAlgoTaskCommand>
{
    public CreateAlgoTaskCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(4, 100);
        RuleFor(c => c.Description).NotEmpty().Length(4, 750);
        RuleFor(c => c.AlgoCategoryId).NotEmpty();
        RuleFor(c => c.InitialCodeSnippet).NotNull().SetValidator(new CodeSnippetValidator());
    }
}