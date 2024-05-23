using FluentValidation;

namespace IQP.Application.Usecases.Questions.Update;

public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(10, 100);
        RuleFor(c => c.Description).NotEmpty().Length(20, 600);
        RuleFor(c => c.CategoryId).NotEmpty();
    }
}