using System.ComponentModel.DataAnnotations;
using IQP.Application.Contracts.Categories.Commands;
using IQP.Application.Contracts.Categories.Responses;
using IQP.Application.Services.Validators;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Services;

public class CategoriesService : ICategoriesService
{
    private readonly IqpDbContext _db;
    private readonly CreateCategoryCommandValidator _createCategoryCommandValidator;
    private readonly UpdateCategoryCommandValidator _updateCategoryCommandValidator;
    private readonly ILogger<CategoriesService> _logger;
    private readonly ICurrentUserService _currentUser;
    private readonly IUserService _userService;

    public CategoriesService(IqpDbContext db, CreateCategoryCommandValidator createCommandValidator, UpdateCategoryCommandValidator updateCategoryCommandValidator, ILogger<CategoriesService> logger, ICurrentUserService currentUser, IUserService userService)
    {
        _db = db;
        _createCategoryCommandValidator = createCommandValidator;
        _updateCategoryCommandValidator = updateCategoryCommandValidator;
        _logger = logger;
        _currentUser = currentUser;
        _userService = userService;
    }

    public async Task<CategoryResponse> CreateCategory(CreateCategoryCommand command)
    {
        if (await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }
        
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var commandValidationResult = _createCategoryCommandValidator.Validate(command);

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

    public async Task<IEnumerable<CategoryResponse>> GetCategories()
    {
        var categories = await _db.Categories
            .Select(c => c.ToResponse())
            .ToListAsync();

        return categories;
    }

    public async Task<CategoryResponse> GetCategoryById(Guid id)
    {
        var category = await _db.Categories.FindAsync(id);
        
        if (category is null)
        {
            throw new IqpException(
                EntityName.Category,Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }
        
        return category.ToResponse();
    }

    public async Task<CategoryResponse> UpdateCategory(UpdateCategoryCommand command)
    {
        if (await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }
        
        var commandValidationResult = _updateCategoryCommandValidator.Validate(command);
        
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

    public async Task<CategoryResponse> DeleteCategory(Guid id)
    {
        if (await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }

        var category = await _db.Categories.FindAsync(id);
        
        if (category is null)
        {
            throw new IqpException(
                EntityName.Category,Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }
        
        _db.Remove(category);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Category with id {CategoryId} has been deleted", category.Id);
        
        return category.ToResponse();
    }
}