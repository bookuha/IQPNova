using FluentValidation;

namespace IQP.Application.Usecases.AlgoCategories.Update;

public class UpdateAlgoTaskCategoryCommandValidator : AbstractValidator<UpdateAlgoTaskCategoryCommand>
{
    public UpdateAlgoTaskCategoryCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Title).NotEmpty().Length(4, 30);
        RuleFor(c => c.Description).NotEmpty().Length(4, 120);
    }
}