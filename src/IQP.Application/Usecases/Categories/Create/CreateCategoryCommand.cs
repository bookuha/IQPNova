using FluentValidation;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
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

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateCategoryCommand> _validator;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;
    
    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoriesRepository categoriesRepository, IUserService userService, ICurrentUserService currentUser, IValidator<CreateCategoryCommand> validator, ILogger<CreateCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _categoriesRepository = categoriesRepository;
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
        
        var titleAlreadyExists = await _categoriesRepository.TitleExistsAsync(command.Title, cancellationToken);

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

        _categoriesRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Category with id {CategoryId}, {Title} has been created", category.Id, category.Title);
        
        return category.ToResponse();
    }
}