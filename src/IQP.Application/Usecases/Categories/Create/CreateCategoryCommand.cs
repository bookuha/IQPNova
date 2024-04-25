using FluentValidation;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.Categories.Create;

public record CreateCategoryCommand : IRequest<CategoryResponse>
{
    public required string Title { get; init; }
    public required string Description { get; init; }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly IqpDbContext _db;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateCategoryCommand> _validator;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;


    public CreateCategoryCommandHandler(IqpDbContext db, IUserService userService, ICurrentUserService currentUser, IValidator<CreateCategoryCommand> validator, ILogger<CreateCategoryCommandHandler> logger)
    {
        _db = db;
        _userService = userService;
        _currentUser = currentUser;
        _validator = validator;
        _logger = logger;
    }

    public async Task<CategoryResponse> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }
        
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var commandValidationResult = _validator.Validate(command);

        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.Category, commandValidationResult.ToDictionary());
        }
        
        var titleAlreadyExists = _db.Categories.Any(c => c.Title == command.Title);

        if (titleAlreadyExists)
        {
            throw new IqpException(
                EntityName.Category,Errors.AlreadyExists.ToString(), "Already exists", "The category with such title already exists.");
        }

        var category = new Category
        {
            Title = command.Title,
            Description = command.Description
        };

        _db.Add(category);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Category with id {CategoryId}, {Title} has been created", category.Id, category.Title);
        
        return category.ToResponse();
    }
}