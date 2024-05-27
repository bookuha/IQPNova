using FluentValidation;

namespace IQP.Application.Usecases.Questions.Comment;

public class CreateCommentaryCommandValidator : AbstractValidator<CreateCommentaryCommand>
{
    public CreateCommentaryCommandValidator()
    {
        RuleFor(c=>c.QuestionId).NotEmpty();
        RuleFor(c => c.Content).MaximumLength(2000);
    }
}