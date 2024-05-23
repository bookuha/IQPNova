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

namespace IQP.Application.Usecases.Categories.Delete;

public record DeleteCategoryCommand : IRequest<CategoryResponse>
{
    public required Guid Id { get; init; }
}

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, CategoryResponse>
{
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(IUnitOfWork unitOfWork, ICategoriesRepository categoriesRepository, IUserService userService, ICurrentUserService currentUser, ILogger<DeleteCategoryCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _categoriesRepository = categoriesRepository;
        _userService = userService;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<CategoryResponse> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        if (!await _userService.IsUserAdmin(_currentUser.UserId.Value))
        {
            throw IqpException.NotAdmin();
        }

        var category = await _categoriesRepository.GetByIdAsync(command.Id, cancellationToken);
        
        if (category is null)
        {
            throw new IqpException(
                EntityName.Category,Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }
        
        _categoriesRepository.Delete(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Category with id {CategoryId} has been deleted", category.Id);
        
        return category.ToResponse();
    }
}