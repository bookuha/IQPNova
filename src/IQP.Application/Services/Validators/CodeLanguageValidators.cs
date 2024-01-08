using FluentValidation;
using IQP.Application.Contracts.CodeLanguages.Commands;

namespace IQP.Application.Services.Validators;

public static class ValidationConstants // Might be shared between validators/services later, therefore might get partial.
{
    public const string FileExtensionRegex = @"\.[a-z]+$"; // eg. .cs, .js, .py
}


public class CreateCodeLanguageCommandValidator : AbstractValidator<CreateCodeLanguageCommand>
{
    public CreateCodeLanguageCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Extension).NotEmpty().MaximumLength(10).Matches(ValidationConstants.FileExtensionRegex);
    }
}

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