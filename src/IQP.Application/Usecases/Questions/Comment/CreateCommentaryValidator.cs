using FluentValidation;

namespace IQP.Application.Usecases.Commentaries.Create;

public class CreateCommentaryCommandValidator : AbstractValidator<CreateCommentaryCommand>
{
    public CreateCommentaryCommandValidator()
    {
        RuleFor(c=>c.QuestionId).NotEmpty();
        RuleFor(c => c.Content).MaximumLength(500);
    }
}