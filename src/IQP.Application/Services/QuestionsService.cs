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

        var category = await _db.Categories.FindAsync(command.CategoryId);
        
        if (category is null)
        {
            throw new IqpException(
                $"{EntityName.Question}.{EntityName.Category}", Errors.NotFound.ToString(), "Category not found",
                "The category with such id does not exist. Therefore question cannot be created.");
        }
        
        var user = await _db.Users.FindAsync(_currentUser.UserId.Value); // TODO: Add null check here / do something at all

        var question = new Question
        {
            Title = command.Title,
            Description = command.Description,
            Creator = user,
            CategoryId = command.CategoryId
        };

        _db.Add(question);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Question with id {QuestionId}, {Title} has been created", question.Id, question.Title);

        return question.ToResponse();
    }

    public async Task<IEnumerable<QuestionResponse>> GetQuestions()
    {
        var query = _db.Questions
            .Include(q => q.Category)
            .Include(q => q.LikedBy)
            .Include(q => q.Commentaries)
            .Include(q => q.Creator);
        
        if (!_currentUser.IsAuthenticated)
        {
            return await query
                .Select(q => q.ToResponse(false))
                .AsSplitQuery()
                .ToListAsync();
        }
        
        return await query
            .Select(q => q.ToResponse(q.LikedBy.Any(u => u.Id == _currentUser.UserId)))
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<QuestionResponse> GetQuestionById(Guid id)
    {
        var question = await 
            _db.Questions
                .Include(q=>q.Category)        
                .Include(q=>q.LikedBy)
                .Include(q=>q.Commentaries)
                .Include(q=>q.Creator)
            .SingleOrDefaultAsync(q => q.Id == id);

        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }

        if (!_currentUser.IsAuthenticated) return question.ToResponse();
        var isLiked = question.LikedBy.Any(u => u.Id == _currentUser.UserId);
        return question.ToResponse(isLiked);
    }

    public async Task<QuestionResponse> UpdateQuestion(UpdateQuestionCommand command)
    {
        var commandValidationResult = _updateQuestionCommandValidator.Validate(command);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.Question, commandValidationResult.ToDictionary());
        }

        var category = await _db.Categories.FindAsync(command.CategoryId);

        if (category is null)
        {
            throw new IqpException(
                $"{EntityName.Question}.{EntityName.Category}", Errors.NotFound.ToString(), "Category not found",
                "The category with such id does not exist. Therefore question cannot be updated.");
        }

        var question = await 
            _db.Questions
                .Include(q=>q.Category)
                .Include(q=>q.LikedBy)
                .Include(q=>q.Commentaries)
                .Include(q=>q.Creator)
                .SingleOrDefaultAsync(q => q.Id == command.Id);
        
        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }
        
        if (_currentUser.UserId != question.CreatorId)
        {
            throw new IqpException(
                EntityName.Question, Errors.Restricted.ToString(), "Restricted",
                "You are not allowed to delete this question.");
        }

        question.Title = command.Title;
        question.Description = command.Description;
        question.CategoryId = command.CategoryId;
        
        _db.Update(question);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Question with id {QuestionId} has been updated", question.Id);

        var isLiked = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        return question.ToResponse(isLiked);
        
    }

    public async Task<QuestionResponse> DeleteQuestion(Guid id)
    {
        var question = await 
            _db.Questions
                .Include(q=>q.Category)
                .Include(q=>q.LikedBy)
                .Include(q=>q.Commentaries)
                .Include(q=>q.Creator)
                .SingleOrDefaultAsync(q => q.Id == id);
        
        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }

        if (_currentUser.UserId != question.CreatorId)
        {
            throw new IqpException(
                EntityName.Question, Errors.Restricted.ToString(), "Restricted",
                "You are not allowed to delete this question.");
        }

        if (question.Commentaries.Any())
        {
            throw new IqpException(
                EntityName.Question, Errors.Restricted.ToString(), "Restricted", "You are not allowed to delete this question because it already has commentaries.");
        }

        _db.Remove(question);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Question with id {QuestionId} has been deleted", question.Id);
        
        var isLiked = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        return question.ToResponse(isLiked);
    }

    public async Task<QuestionResponse> LikeQuestion(Guid id)
    {
        var question = await 
            _db.Questions
                .Include(q=>q.Category)
                .Include(q=>q.LikedBy)
                .Include(q=>q.Commentaries)
                .Include(q=>q.Creator)
                .SingleOrDefaultAsync(q => q.Id == id);

        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }
        
        var currentUser = await _db.Users.FindAsync(_currentUser.UserId);

        var isLikedAlready = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        if (isLikedAlready)
        {
            question.LikedBy.Remove(currentUser);
        }
        else
        {
            question.LikedBy.Add(currentUser);
        }
        await _db.SaveChangesAsync();
        
        var isLiked = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        return question.ToResponse(isLiked);
    }

    // public async Task<CommentaryResponse> CreateCommentary(CreateCommentaryCommand command)
    
    // public async Task<CommentaryResponse> UpdateCommentary(UpdateCommentaryCommand command) // Not that important for now
    
    // public async Task<CommentaryResponse> DeleteCommentary(Guid id)
}