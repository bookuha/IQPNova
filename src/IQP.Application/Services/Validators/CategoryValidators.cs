using FluentValidation;
using IQP.Application.Contracts.Categories.Commands;

namespace IQP.Application.Services.Validators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(4, 30);
        RuleFor(c => c.Description).NotEmpty().Length(4, 120);
    }
}

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(4, 30);
        RuleFor(c => c.Description).NotEmpty().Length(4, 120);
    }
}