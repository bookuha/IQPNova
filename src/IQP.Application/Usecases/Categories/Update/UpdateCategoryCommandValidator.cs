using FluentValidation;

namespace IQP.Application.Usecases.Categories.Update;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(4, 30);
        RuleFor(c => c.Description).NotEmpty().Length(4, 120);
    }
}