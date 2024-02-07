using FluentValidation;
using IQP.Application.Contracts.Questions.Commands;

namespace IQP.Application.Services.Validators;

public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(10, 100);
        RuleFor(c => c.Description).NotEmpty().Length(20, 600);
        RuleFor(c => c.CategoryId).NotEmpty();
    }
}

public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(10, 100);
        RuleFor(c => c.Description).NotEmpty().Length(20, 600);
        RuleFor(c => c.CategoryId).NotEmpty();
    }
}