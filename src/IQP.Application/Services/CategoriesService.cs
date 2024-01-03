using System.ComponentModel.DataAnnotations;
using IQP.Application.Contracts.Categories.Commands;
using IQP.Application.Contracts.Categories.Responses;
using IQP.Application.Services.Validators;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Services;

public class CategoriesService : ICategoriesService
{
    private readonly IqpDbContext _db;
    private readonly CreateCategoryCommandValidator _createQuestionCommandValidator;
    private readonly UpdateCategoryCommandValidator _updateCategoryCommandValidator;
    private readonly ILogger<CategoriesService> _logger;

    public CategoriesService(IqpDbContext db, CreateCategoryCommandValidator createCommandValidator, UpdateCategoryCommandValidator updateCategoryCommandValidator, ILogger<CategoriesService> logger)
    {
        _db = db;
        _createQuestionCommandValidator = createCommandValidator;
        _updateCategoryCommandValidator = updateCategoryCommandValidator;
        _logger = logger;
    }

    public async Task<CategoryResponse> CreateCategory(CreateCategoryCommand command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var commandValidationResult = _createQuestionCommandValidator.Validate(command);

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