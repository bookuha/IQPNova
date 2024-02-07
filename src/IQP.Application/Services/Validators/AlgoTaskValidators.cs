using FluentValidation;
using IQP.Application.Contracts.AlgoTasks.Commands;
using IQP.Application.Contracts.AlgoTasks.Utility;
using IQP.Application.Services.Validators;

namespace IQP.Application.Services.Validators;

public class SubmitAlgoTaskSolutionCommandValidator : AbstractValidator<SubmitAlgoTaskSolutionCommand>
{
    public SubmitAlgoTaskSolutionCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.AlgoTaskId).NotEmpty();
        RuleFor(c => c.LanguageId).NotEmpty();
    }
    
}

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

public class RunTestsOnCodeCommandValidator : AbstractValidator<RunTestsOnCodeCommand>
{
    public RunTestsOnCodeCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.Tests).NotEmpty().MaximumLength(2000);
        RuleFor(c => c.LanguageId).NotEmpty();
    }
}

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

public class UpdateAlgoTaskCommandValidator : AbstractValidator<UpdateAlgoTaskCommand>
{
    public UpdateAlgoTaskCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(4, 100);
        RuleFor(c => c.Description).NotEmpty().Length(4, 750);
        RuleFor(c => c.AlgoCategoryId).NotEmpty();
    }
}

public class AddNewLanguageToAlgoTaskCommandValidator : AbstractValidator<AddNewLanguageToAlgoTaskCommand>
{
    public AddNewLanguageToAlgoTaskCommandValidator()
    {
        RuleFor(c => c.AlgoTaskId).NotEmpty();
        RuleFor(c => c.InitialCodeSnippet).NotNull().SetValidator(new CodeSnippetValidator());
    }
}