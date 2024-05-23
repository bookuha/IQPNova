using FluentValidation;

namespace IQP.Application.Usecases.Questions.Create;

public class CreateQuestionCommandValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(10, 100);
        RuleFor(c => c.Description).NotEmpty().Length(20, 600);
        RuleFor(c => c.CategoryId).NotEmpty();
    }
}