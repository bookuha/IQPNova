using FluentValidation;

namespace IQP.Application.Usecases.AlgoCategories.Create;

public class CreateAlgoTaskCategoryCommandValidator : AbstractValidator<CreateAlgoTaskCategoryCommand>
{
    public CreateAlgoTaskCategoryCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(4, 30);
        RuleFor(c => c.Description).NotEmpty().Length(4, 120);
    }
}