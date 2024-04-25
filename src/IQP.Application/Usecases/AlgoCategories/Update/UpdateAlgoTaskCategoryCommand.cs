using FluentValidation;
using IQP.Application.Contracts.AlgoTaskCategories.Responses;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.AlgoCategories.Update;

public record UpdateAlgoTaskCategoryCommand : IRequest<AlgoTaskCategoryResponse>
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
}

public class UpdateAlgoTaskCategoryCommandHandler : IRequestHandler<UpdateAlgoTaskCategoryCommand, AlgoTaskCategoryResponse>
{
    private readonly IqpDbContext _db;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<UpdateAlgoTaskCategoryCommand> _validator;
    private readonly ILogger<UpdateAlgoTaskCategoryCommandHandler> _logger;

    public UpdateAlgoTaskCategoryCommandHandler(IqpDbContext db, IUserService userService, ICurrentUserService currentUser, IValidator<UpdateAlgoTaskCategoryCommand> validator, ILogger<UpdateAlgoTaskCategoryCommandHandler> logger)
    {
        _db = db;
        _userService = userService;
        _currentUser = currentUser;
        _validator = validator;
        _logger = logger;
    }

    public async Task<AlgoTaskCategoryResponse> Handle(UpdateAlgoTaskCategoryCommand command, CancellationToken cancellationToken)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }

        var commandValidationResult = _validator.Validate(command);

        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoCategory, commandValidationResult.ToDictionary());
        }

        var category = await _db.AlgoTaskCategories.FindAsync(command.Id);

        if (category is null)
        {
            throw new IqpException(EntityName.AlgoCategory, Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }

        category.Title = command.Title;
        category.Description = command.Description;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Category with id {CategoryId}, {Title} has been updated", category.Id, category.Title);

        return category.ToResponse();
    }
}