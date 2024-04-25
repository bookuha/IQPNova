using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.Questions.Get;

public record GetQuestionsQuery : IRequest<IEnumerable<QuestionResponse>>
{
    
}

public class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, IEnumerable<QuestionResponse>>
{

    private readonly IQuestionsRepository _questionsRepository;
    private readonly ICurrentUserService _currentUser;

    public GetQuestionsQueryHandler(IQuestionsRepository questionsRepository, ICurrentUserService currentUser)
    {
        _questionsRepository = questionsRepository;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<QuestionResponse>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await _questionsRepository.GetAsync(cancellationToken);
        if (!_currentUser.IsAuthenticated)
        {
            return questions
                .Select(q => q.ToResponse(false));
        }

        return questions
            .Select(q => q.ToResponse(q.LikedBy.Any(u => u.Id == _currentUser.UserId)));
    }
}