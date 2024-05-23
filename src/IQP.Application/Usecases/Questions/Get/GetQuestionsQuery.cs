using IQP.Domain.Entities;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Services;
using IQP.Shared;
using MediatR;

namespace IQP.Application.Usecases.Questions.Get;

public record GetQuestionsQuery(string? SearchTerm, Guid? CategoryId, string? SortColumn, string? SortOrder, int Page, int PageSize) : IRequest<PagedList<QuestionResponse>>
{
    
}

public class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, PagedList<QuestionResponse>>
{

    private readonly IQuestionsRepository _questionsRepository;
    private readonly ICurrentUserService _currentUser;

    public GetQuestionsQueryHandler(IQuestionsRepository questionsRepository, ICurrentUserService currentUser)
    {
        _questionsRepository = questionsRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedList<QuestionResponse>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questions = await _questionsRepository.GetAsync(request.SearchTerm, request.CategoryId, request.SortColumn, request.SortOrder, request.Page, request.PageSize, cancellationToken);
        if (!_currentUser.IsAuthenticated)
        {
            return questions.Map(q => q.ToResponse());
        }

        return questions.Map(q => q.ToResponse(q.LikedBy.Any(u => u.Id == _currentUser.UserId)));
    }
}