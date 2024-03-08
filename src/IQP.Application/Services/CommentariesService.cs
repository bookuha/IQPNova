using IQP.Application.Contracts.Commentaries;
using IQP.Application.Contracts.Commentaries.Commands;
using IQP.Application.Services.Validators;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IQP.Application.Services;

public class CommentariesService : ICommentariesService
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly CreateCommentaryCommandValidator _createCommentaryCommandValidator;
    private readonly UpdateCommentaryCommandValidator _updateCommentaryCommandValidator;
    private ILogger<CommentariesService> _logger;


    public CommentariesService(IqpDbContext db, ICurrentUserService currentUser, CreateCommentaryCommandValidator createCommentaryCommandValidator, UpdateCommentaryCommandValidator updateCommentaryCommandValidator, ILogger<CommentariesService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _createCommentaryCommandValidator = createCommentaryCommandValidator;
        _updateCommentaryCommandValidator = updateCommentaryCommandValidator;
        _logger = logger;
    }

    public async Task<CommentaryResponse> CreateCommentary(CreateCommentaryCommand command)
    {
        var commandValidationResult = _createCommentaryCommandValidator.Validate(command);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.Commentary, commandValidationResult.ToDictionary());
        }
        
        var question = await _db.Questions.FindAsync(command.QuestionId);
        
        if (question is null)
        {
            throw new IqpException(
                $"{EntityName.Commentary}.{EntityName.Question}", Errors.NotFound.ToString(), "Question not found",
                "The question with such id does not exist. Therefore commentary cannot be created.");
        }

        var currentUser = await _db.Users.FindAsync(_currentUser.UserId.Value);
        
        if (command.ReplyToId is not null) // Means it is not a root commentary
        {
            var root = await _db.Commentaries.FindAsync(command.ReplyToId);
            
            if (root is null)
            {
                throw new IqpException(
                    EntityName.Commentary, Errors.NotFound.ToString(), "Commentary not found",
                    "The commentary with such id does not exist. Therefore commentary cannot be created.");
            }

            // Note: I want to allow only 1 level of replies.
            
            var actualRootId = root.ReplyToId ?? root.Id; // Therefore, If specified commentary is a reply, then redirect the new commentary to its root.

            var reply = new Commentary
            {
                Content = command.Content,
                Question = question,
                CreatedBy = currentUser,
                ReplyToId = actualRootId
            };

            _db.Commentaries.Add(reply);

            await _db.SaveChangesAsync();

            return reply.ToResponse();
        }

        var commentary = new Commentary
        {
            Content = command.Content,
            Question = question,
            CreatedBy = currentUser
        };

        _db.Commentaries.Add(commentary);
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Commentary with id {CommentaryId}, {Content} has been created", commentary.Id, commentary.Content);
        
        return commentary.ToResponse();
    }

    public async Task<IEnumerable<CommentaryResponse>> GetCommentariesByQuestionId(Guid questionId)
    {
        // Warning! When using split queries with Skip/Take, pay special attention to making your query ordering fully unique...

        var query = _db.Commentaries
            .Where(c => c.QuestionId == questionId)
            .Include(c => c.Replies)
            .ThenInclude(r => r.CreatedBy)
            .Include(c => c.LikedBy)
            .Include(c => c.CreatedBy)
            .Where(c => c.ReplyTo == null);

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

    public Task<CommentaryResponse> UpdateCommentary(UpdateCommentaryCommand command)
    {
        throw new NotImplementedException();
    }

    public Task<CommentaryResponse> DeleteCommentary(Guid id)
    {
        throw new NotImplementedException();
    }
}