using FluentValidation;
using IQP.Application.Contracts.Questions.Commands;
using IQP.Application.Contracts.Questions.Responses;
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

public class QuestionsService : IQuestionsService
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly CreateQuestionCommandValidator _createQuestionCommandValidator;
    private readonly UpdateQuestionCommandValidator _updateQuestionCommandValidator;
    private ILogger<QuestionsService> _logger;

    public QuestionsService(IqpDbContext db, ICurrentUserService currentUser, CreateQuestionCommandValidator createQuestionCommandValidator, UpdateQuestionCommandValidator updateQuestionCommandValidator, ILogger<QuestionsService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _createQuestionCommandValidator = createQuestionCommandValidator;
        _updateQuestionCommandValidator = updateQuestionCommandValidator;
        _logger = logger;
    }

    public async Task<QuestionResponse> CreateQuestion(CreateQuestionCommand command)
    {
        var commandValidationResult = _createQuestionCommandValidator.Validate(command);

        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.Question, commandValidationResult.ToDictionary());
        }
        
        // TODO: Add similar question title check here

        var category = await _db.Categories.FindAsync(command.CategoryId);
        
        if (category is null)
        {
            throw new IqpException(
                $"{EntityName.Question}.{EntityName.Category}", Errors.NotFound.ToString(), "Category not found",
                "The category with such id does not exist. Therefore question cannot be created.");
        }
        
        var question = new Question
        {
            Title = command.Title,
            Description = command.Description,
            CreatorId = _currentUser.UserId.Value,
            CategoryId = command.CategoryId
        };
        
        _db.Add(question);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Question with id {QuestionId}, {Title} has been created", question.Id, question.Title);

        return question.ToResponse();
    }

    public async Task<IEnumerable<QuestionResponse>> GetQuestions()
    {
        return await _db.Questions.Select(q => q.ToResponse()).ToListAsync();
    }

    public async Task<QuestionResponse> GetQuestionById(Guid id)
    {
        var question = await _db.Questions.FindAsync(id);

        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }
        
        return question.ToResponse();
    }

    public async Task<QuestionResponse> UpdateQuestion(UpdateQuestionCommand command)
    {
        var commandValidationResult = _updateQuestionCommandValidator.Validate(command);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.Question, commandValidationResult.ToDictionary());
        }

        // Prevent updating if the user is not the author of the question.
        
        var question = await _db.Questions.FindAsync(command.Id);
        
        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }
        
        question.Title = command.Title;
        question.Description = command.Description;
        question.CategoryId = command.CategoryId;
        
        _db.Update(question);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Question with id {QuestionId} has been updated", question.Id);

        return question.ToResponse();
        
    }

    public async Task<QuestionResponse> DeleteQuestion(Guid id)
    {
        var question = await _db.Questions.FindAsync(id);
        
        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }
        
        // Check if author is the same as the current user. If not, prevent deletion.
        // Check if there are any commentaries. If so, prevent deletion.

        _db.Remove(question);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Question with id {QuestionId} has been deleted", question.Id);
        
        return question.ToResponse();
    }

    public async Task<QuestionResponse> LikeQuestion(Guid id)
    {
        var question = await _db.Questions.FindAsync(id);

        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }

        if (question.LikedBy.Any(user => user.Id == _currentUser.UserId))
        {
            throw new IqpException(
                EntityName.Question,Errors.WrongFlow.ToString(), "Wrong flow", "You have already liked this question. Maybe you want to undo the like?");
        }

        var currentUser = await _db.Users.FindAsync(_currentUser.UserId);
        if (currentUser is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.Critical.ToString(), "Critical", "Critical authorization error has just occured.");
        }

        question.LikedBy.Add(currentUser);

        await _db.SaveChangesAsync();

        return question.ToResponse();
    }
    
    // public async Task<QuestionResponse> UndoLikeQuestion(Guid id) // Check how its done on YT
    
    // public async Task<CommentaryResponse> CreateCommentary(CreateCommentaryCommand command)
    
    // public async Task<CommentaryResponse> UpdateCommentary(UpdateCommentaryCommand command) // Not that important for now
    
    // public async Task<CommentaryResponse> DeleteCommentary(Guid id)
}