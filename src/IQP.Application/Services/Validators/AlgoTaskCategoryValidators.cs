using FluentValidation;
using IQP.Application.Contracts.AlgoTaskCategories.Commands;

namespace IQP.Application.Services.Validators;

public class CreateAlgoTaskCategoryCommandValidator : AbstractValidator<CreateAlgoTaskCategoryCommand>
{
    public CreateAlgoTaskCategoryCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(4, 30);
        RuleFor(c => c.Description).NotEmpty().Length(4, 120);
    }
}

public class UpdateAlgoTaskCategoryCommandValidator : AbstractValidator<UpdateAlgoTaskCategoryCommand>
{
    public UpdateAlgoTaskCategoryCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Title).NotEmpty().Length(4, 30);
        RuleFor(c => c.Description).NotEmpty().Length(4, 120);
    }
}