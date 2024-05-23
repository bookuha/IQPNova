using FluentValidation;

namespace IQP.Application.Usecases.AlgoTasks;

public class CodeSnippetValidator : AbstractValidator<CodeSnippet>
{
    public CodeSnippetValidator()
    {
        RuleFor(c => c.InitialSolutionCode).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.SampleCode).NotEmpty().MaximumLength(1000);
        RuleFor(c => c.TestsCode).NotEmpty().MaximumLength(3000);
        RuleFor(c => c.LanguageId).NotEmpty();
    }
}