using IQP.Application.Contracts.AlgoTaskCategories.Responses;
using IQP.Application.Services;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Infrastructure.Data;
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
    private readonly IqpDbContext _db;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<DeleteAlgoTaskCategoryCommandHandler> _logger;


    public DeleteAlgoTaskCategoryCommandHandler(IqpDbContext db, IUserService userService, ICurrentUserService currentUser, ILogger<DeleteAlgoTaskCategoryCommandHandler> logger)
    {
        _db = db;
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

        var category = await _db.AlgoTaskCategories.FindAsync(command.Id);

        if (category is null)
        {
            throw new IqpException(EntityName.AlgoCategory, Errors.NotFound.ToString(), "Not found", "The category with such id does not exist.");
        }

        _db.Remove(category);

        await _db.SaveChangesAsync();

        _logger.LogInformation("Category with id {CategoryId}, {Title} has been deleted", category.Id, category.Title);

        return category.ToResponse();
    }
}
