using FluentValidation;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.Questions.Comment;

public record CreateCommentaryCommand : IRequest<CommentaryResponse>
{
    public required Guid QuestionId { get; set; }
    public required string Content { get; set; }
    public Guid? ReplyToId { get; set; }
}

public class CreateCommentaryCommandHandler : IRequestHandler<CreateCommentaryCommand, CommentaryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<CreateCommentaryCommand> _validator;
    private readonly ILogger<CreateCommentaryCommandHandler> _logger;


    public CreateCommentaryCommandHandler(IUnitOfWork unitOfWork, IQuestionsRepository questionsRepository, IUserService userService, ICurrentUserService currentUser, IValidator<CreateCommentaryCommand> validator, ILogger<CreateCommentaryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _questionsRepository = questionsRepository;
        _userService = userService;
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
        
        var question = await _questionsRepository.GetByIdAsync(command.QuestionId);
        
        if (question is null)
        {
            throw new IqpException(
                $"{EntityName.Commentary}.{EntityName.Question}", Errors.NotFound.ToString(), "Question not found",
                "The question with such id does not exist. Therefore commentary cannot be created.");
        }

        var currentUser = await _userService.GetUserByIdAsync(_currentUser.UserId.Value);
        
        var commentary = question.Comment(command.Content, currentUser, command.ReplyToId);
        _questionsRepository.AddCommentary(question, commentary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Commentary with id {CommentaryId}, {Content} has been created", commentary.Id, commentary.Content);
        
        return commentary.ToResponse();
    }
}
