using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IQP.Application.Usecases.AlgoTasks.GetById;

public record GetAlgoTaskByIdQuery : IRequest<AlgoTaskResponse>
{
    public Guid Id { get; set; }
}

public class GetAlgoTaskByIdQueryHandler : IRequestHandler<GetAlgoTaskByIdQuery, AlgoTaskResponse>
{
    private readonly IAlgoTasksRepository _algoTasksRepository;
    private readonly ICurrentUserService _currentUser;
    
    public GetAlgoTaskByIdQueryHandler(IAlgoTasksRepository algoTasksRepository, ICurrentUserService currentUser)
    {
        _algoTasksRepository = algoTasksRepository;
        _currentUser = currentUser;
    }

    public async Task<AlgoTaskResponse> Handle(GetAlgoTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var algoTask = await _algoTasksRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (algoTask is null)
        {
            throw new IqpException(
                EntityName.AlgoTask,Errors.NotFound.ToString(), "Not found", "The algorithm task with such id does not exist.");
        }

        if (!_currentUser.IsAuthenticated) return algoTask.ToResponse(Functions.GetTaskSupportedLanguages(algoTask), algoTask.CodeSnippets, false);
        var isPassed = algoTask.PassedBy.Any(u => u.Id == _currentUser.UserId);
        return algoTask.ToResponse(Functions.GetTaskSupportedLanguages(algoTask), algoTask.CodeSnippets, isPassed);
    }
}

