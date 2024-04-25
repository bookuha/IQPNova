using FluentValidation;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.Categories.Update;

public record UpdateCategoryCommand : IRequest<CategoryResponse>
{
    public Guid Id { get; set; } // TODO: Why not required?
    public required string Title { get; set; }
    public required string Description { get; set; }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    private readonly IqpDbContext _db;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<UpdateCategoryCommand> _validator;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(IqpDbContext db, IUserService userService, ICurrentUserService currentUser, IValidator<UpdateCategoryCommand> validator, ILogger<UpdateCategoryCommandHandler> logger)
    {
        _db = db;
        _userService = userService;
        _currentUser = currentUser;
        _validator = validator;
        _logger = logger;
    }

    public async Task<CategoryResponse> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }
        
        var commandValidationResult = _validator.Validate(command);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.Category, commandValidationResult.ToDictionary());
        }
        
        var category = await _db.Categories.FindAsync(command.Id);
        
        if (category is null)
        {
            throw new IqpException(
                EntityName.Category,Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }
        
        category.Title = command.Title;
        category.Description = command.Description;
        
        _db.Update(category);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Category with id {CategoryId} has been updated", category.Id);
        
        return category.ToResponse();
    }
}