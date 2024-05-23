using IQP.Application.Contracts.AlgoTaskCategories.Responses;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IQP.Application.Usecases.AlgoCategories.Delete;

public record DeleteAlgoTaskCategoryCommand : IRequest<AlgoTaskCategoryResponse>
{
    public required Guid Id { get; init; }
}

public class DeleteAlgoTaskCategoryCommandHandler : IRequestHandler<DeleteAlgoTaskCategoryCommand, AlgoTaskCategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAlgoCategoriesRepository _algoCategoriesRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteAlgoTaskCategoryCommandHandler> _logger;
    
    public DeleteAlgoTaskCategoryCommandHandler(IUnitOfWork unitOfWork, IAlgoCategoriesRepository algoCategoriesRepository, IUserService userService, ICurrentUserService currentUser, ILogger<DeleteAlgoTaskCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _algoCategoriesRepository = algoCategoriesRepository;
        _userService = userService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<AlgoTaskCategoryResponse> Handle(DeleteAlgoTaskCategoryCommand command, CancellationToken cancellationToken)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }

        var category = await _algoCategoriesRepository.GetByIdAsync(command.Id, cancellationToken);

        if (category is null)
        {
            throw new IqpException(EntityName.AlgoCategory, Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }

        _algoCategoriesRepository.Delete(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category with id {CategoryId}, {Title} has been deleted", category.Id, category.Title);

        return category.ToResponse();
    }
}
