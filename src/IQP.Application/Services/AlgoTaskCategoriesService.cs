using IQP.Application.Contracts.AlgoTaskCategories.Commands;
using IQP.Application.Contracts.AlgoTaskCategories.Responses;
using IQP.Application.Services.Validators;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IQP.Application.Services;

public class AlgoTaskCategoriesService : IAlgoTaskCategoriesService
{
    private readonly IqpDbContext _db;
    private readonly CreateAlgoTaskCategoryCommandValidator _createAlgoTaskCategoryCommandValidator;
    private readonly UpdateAlgoTaskCategoryCommandValidator _updateAlgoTaskCategoryCommandValidator;
    private readonly ILogger<AlgoTaskCategoriesService> _logger;

    public AlgoTaskCategoriesService(IqpDbContext db, CreateAlgoTaskCategoryCommandValidator createAlgoTaskCategoryCommandValidator, UpdateAlgoTaskCategoryCommandValidator updateAlgoTaskCategoryCommandValidator, ILogger<AlgoTaskCategoriesService> logger)
    {
        _db = db;
        _createAlgoTaskCategoryCommandValidator = createAlgoTaskCategoryCommandValidator;
        _updateAlgoTaskCategoryCommandValidator = updateAlgoTaskCategoryCommandValidator;
        _logger = logger;
    }
    
    public async Task<AlgoTaskCategoryResponse> CreateCategory(CreateAlgoTaskCategoryCommand command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var commandValidationResult = _createAlgoTaskCategoryCommandValidator.Validate(command);

        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoCategory, commandValidationResult.ToDictionary());
        }
        
        var titleAlreadyExists = _db.AlgoTaskCategories.Any(c => c.Title == command.Title);

        if (titleAlreadyExists)
        {
            throw new IqpException(
                EntityName.AlgoCategory,Errors.AlreadyExists.ToString(), "Already exists", "The category with such title already exists.");
        }

        var category = new AlgoTaskCategory
        {
            Title = command.Title,
            Description = command.Description
        };

        _db.Add(category);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Category with id {CategoryId}, {Title} has been created", category.Id, category.Title);

        return category.ToResponse();
    }
    
    public async Task<IEnumerable<AlgoTaskCategoryResponse>> GetCategories()
    {
        return await _db.AlgoTaskCategories
            .Select(c => c.ToResponse())
            .ToListAsync();
    }
    
    public async Task<AlgoTaskCategoryResponse> GetCategoryById(Guid id)
    {
        var category = await _db.AlgoTaskCategories.FindAsync(id);

        if (category is null)
        {
            throw new IqpException(EntityName.AlgoCategory, Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }

        return category.ToResponse();
    }
    
    public async Task<AlgoTaskCategoryResponse> UpdateCategory(UpdateAlgoTaskCategoryCommand command)
    {
        var commandValidationResult = _updateAlgoTaskCategoryCommandValidator.Validate(command);

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
    
    public async Task<AlgoTaskCategoryResponse> DeleteCategory(Guid id)
    {
        var category = await _db.AlgoTaskCategories.FindAsync(id);

        if (category is null)
        {
            throw new IqpException(EntityName.AlgoCategory, Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }

        _db.Remove(category);

        await _db.SaveChangesAsync();

        _logger.LogInformation("Category with id {CategoryId}, {Title} has been deleted", category.Id, category.Title);

        return category.ToResponse();
    }
    
}