using FluentValidation;
using IQP.Application.Contracts.Commentaries.Commands;

namespace IQP.Application.Services.Validators;

public class CreateCommentaryCommandValidator : AbstractValidator<CreateCommentaryCommand>
{
    public CreateCommentaryCommandValidator()
    {
        RuleFor(c=>c.QuestionId).NotEmpty();
        RuleFor(c => c.Content).MaximumLength(500);
    }
}

public class UpdateCommentaryCommandValidator : AbstractValidator<UpdateCommentaryCommand>
{
    public UpdateCommentaryCommandValidator()
    {
        RuleFor(c=>c.Id).NotEmpty();
        RuleFor(c => c.Content).MaximumLength(500);
    }
}