using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IQP.Application.Usecases.Questions.Delete;

public record DeleteQuestionCommand : IRequest<QuestionResponse>
{
    public required Guid Id { get; set; }
}

public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, QuestionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteQuestionCommandHandler> _logger;
    
    public DeleteQuestionCommandHandler(IUnitOfWork unitOfWork, IQuestionsRepository questionsRepository, ICurrentUserService currentUser, ILogger<DeleteQuestionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _questionsRepository = questionsRepository;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<QuestionResponse> Handle(DeleteQuestionCommand command, CancellationToken cancellationToken)
    {
        var question = await _questionsRepository.GetByIdAsync(command.Id, cancellationToken);
        
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

        _questionsRepository.Delete(question);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Question with id {QuestionId} has been deleted", question.Id);
        
        var isLiked = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        return question.ToResponse(isLiked);
    }
}