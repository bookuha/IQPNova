using FluentValidation;

namespace IQP.Application.Usecases.AlgoTasks.Update;

public class UpdateAlgoTaskCommandValidator : AbstractValidator<UpdateAlgoTaskCommand>
{
    public UpdateAlgoTaskCommandValidator()
    {
        RuleFor(c => c.Title).NotEmpty().Length(4, 100);
        RuleFor(c => c.Description).NotEmpty().Length(4, 750);
        RuleFor(c => c.AlgoCategoryId).NotEmpty();
    }
}