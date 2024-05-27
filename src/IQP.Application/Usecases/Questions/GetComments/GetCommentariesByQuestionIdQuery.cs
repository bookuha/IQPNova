using IQP.Domain.Repositories;
using IQP.Infrastructure.Services;
using MediatR;

namespace IQP.Application.Usecases.Questions.GetComments;

public record GetCommentariesByQuestionIdQuery : IRequest<IEnumerable<CommentaryResponse>>
{
    public required Guid QuestionId { get; set; }
}

public class
    GetCommentariesByQuestionIdQueryHandler : IRequestHandler<GetCommentariesByQuestionIdQuery,
    IEnumerable<CommentaryResponse>>
{
    private readonly IQuestionsRepository _questionsRepository;
    private readonly ICurrentUserService _currentUser;
    
    public GetCommentariesByQuestionIdQueryHandler(IQuestionsRepository questionsRepository, ICurrentUserService currentUser)
    {
        _questionsRepository = questionsRepository;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<CommentaryResponse>> Handle(GetCommentariesByQuestionIdQuery request,
        CancellationToken cancellationToken)
    {
        var commentaries = await _questionsRepository.GetCommentariesByQuestionIdAsync(request.QuestionId, cancellationToken);

        if (!_currentUser.IsAuthenticated)
        {
            return commentaries.Select(q => q.ToResponse(false));
        }

        return commentaries.Select(q => q.ToResponse(q.LikedBy.Any(u => u.Id == _currentUser.UserId)));
    }
}