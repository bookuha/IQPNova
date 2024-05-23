using FluentValidation;

namespace IQP.Application.Usecases.Categories.Create;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(4, 30);
        RuleFor(c => c.Description).NotEmpty().Length(4, 120);
    }
}