using FluentValidation;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.Commentaries.Create;

public record CreateCommentaryCommand : IRequest<CommentaryResponse>
{
    public required Guid QuestionId { get; set; }
    public required string Content { get; set; }
    public Guid? ReplyToId { get; set; }
}

public class CreateCommentaryCommandHandler : IRequestHandler<CreateCommentaryCommand, CommentaryResponse>
{
    private readonly IqpDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateCommentaryCommand> _validator;
    private readonly ILogger<CreateCommentaryCommandHandler> _logger;


    public CreateCommentaryCommandHandler(IqpDbContext db, ICurrentUserService currentUser, IValidator<CreateCommentaryCommand> validator, ILogger<CreateCommentaryCommandHandler> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _validator = validator;
        _logger = logger;
    }

    public async Task<CommentaryResponse> Handle(CreateCommentaryCommand command, CancellationToken cancellationToken)
    {
      var commandValidationResult = _validator.Validate(command);
        
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
}
