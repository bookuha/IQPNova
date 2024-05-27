using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.Questions.Like;

public record LikeQuestionCommand : IRequest<QuestionResponse>
{
    public required Guid Id { get; init; }
}

public class LikeQuestionCommandHandler : IRequestHandler<LikeQuestionCommand, QuestionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    
    public LikeQuestionCommandHandler(IUnitOfWork unitOfWork, IQuestionsRepository questionsRepository, IUserService userService, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _questionsRepository = questionsRepository;
        _userService = userService;
        _currentUser = currentUser;
    }

    public async Task<QuestionResponse> Handle(LikeQuestionCommand command, CancellationToken cancellationToken)
    {
        var question = await _questionsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }
        
        var currentUser = await _userService.GetUserByIdAsync(_currentUser.UserId.Value);

        question.Like(currentUser!);
        
        _questionsRepository.Update(question);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var isLiked = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        return question.ToResponse(isLiked);
    }
}