using FluentValidation;

namespace IQP.Application.Usecases.CodeLanguages.Create;

public class CreateCodeLanguageCommandValidator : AbstractValidator<CreateCodeLanguageCommand>
{
    public CreateCodeLanguageCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Extension).NotEmpty().MaximumLength(10).Matches(ValidationConstants.FileExtensionRegex);
    }
}