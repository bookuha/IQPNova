using FluentValidation;

namespace IQP.Application.Usecases.CodeLanguages.Update;

public class UpdateCodeLanguageCommandValidator : AbstractValidator<UpdateCodeLanguageCommand>
{
    public UpdateCodeLanguageCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Extension).NotEmpty().MaximumLength(10).Matches(ValidationConstants.FileExtensionRegex);
    }
}