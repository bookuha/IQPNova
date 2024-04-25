using FluentValidation;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.AlgoTasks.Update;

public record UpdateAlgoTaskCommand : IRequest<AlgoTaskResponse>  
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid AlgoCategoryId { get; set; }
}

public class UpdateAlgoTaskCommandHandler : IRequestHandler<UpdateAlgoTaskCommand, AlgoTaskResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAlgoTasksRepository _algoTasksRepository;
    private readonly IAlgoCategoriesRepository _algoCategoriesRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly IValidator<UpdateAlgoTaskCommand> _validator;
    
    public UpdateAlgoTaskCommandHandler(IUnitOfWork unitOfWork, IAlgoTasksRepository algoTasksRepository, IAlgoCategoriesRepository algoCategoriesRepository, IUserService userService, ICurrentUserService currentUser, IValidator<UpdateAlgoTaskCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _algoTasksRepository = algoTasksRepository;
        _algoCategoriesRepository = algoCategoriesRepository;
        _userService = userService;
        _currentUser = currentUser;
        _validator = validator;
    }

    public async Task<AlgoTaskResponse> Handle(UpdateAlgoTaskCommand command, CancellationToken cancellationToken)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }
        
        var commandValidationResult = _validator.Validate(command);

        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.AlgoTask,
                commandValidationResult.ToDictionary());
        }
        
        var algoTask = await _algoTasksRepository.GetByIdAsync(command.Id, cancellationToken);

        if (algoTask is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.NotFound.ToString(), "AlgoTask not found",
                "The algo task with such id does not exist. Therefore update cannot be made.");
        }

        var titleAlreadyExists =
            await _algoTasksRepository.TitleExistsExceptInAsync(command.Title, command.Id, cancellationToken);
        if(titleAlreadyExists)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}", Errors.AlreadyExists.ToString(), "Already exists",
                "The algo task with such title already exists. Therefore update cannot be made.");
        }
        
        var newAlgoCategory = await _algoCategoriesRepository.GetByIdAsync(command.AlgoCategoryId, cancellationToken);
        if(newAlgoCategory is null)
        {
            throw new IqpException(
                $"{EntityName.AlgoTask}.{EntityName.AlgoCategory}", Errors.NotFound.ToString(), "AlgoCategory not found",
                "The algo category with such id does not exist. Therefore update cannot be made.");
        }

        algoTask.Title = command.Title;
        algoTask.Description = command.Description;
        algoTask.AlgoCategoryId = command.AlgoCategoryId;
        
        _algoTasksRepository.Update(algoTask);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var isPassed = algoTask.PassedBy.Any(u => u.Id == _currentUser.UserId);
        return algoTask.ToResponse(Functions.GetTaskSupportedLanguages(algoTask), algoTask.CodeSnippets, isPassed);
    }
} 