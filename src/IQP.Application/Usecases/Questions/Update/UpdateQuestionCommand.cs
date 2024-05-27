using FluentValidation;
using IQP.Application.Services.Users;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Domain.Repositories;
using IQP.Infrastructure.Data;
using IQP.Infrastructure.Repositories;
using IQP.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ValidationException = IQP.Domain.Exceptions.ValidationException;

namespace IQP.Application.Usecases.Questions.Update;

public record UpdateQuestionCommand : IRequest<QuestionResponse>
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required Guid CategoryId { get; set; }
}

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, QuestionResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<UpdateQuestionCommandHandler> _logger;
    private readonly IValidator<UpdateQuestionCommand> _validator;
    
    public UpdateQuestionCommandHandler(IUnitOfWork unitOfWork, IQuestionsRepository questionsRepository, ICategoriesRepository categoriesRepository, IUserService userService, ICurrentUserService currentUser, ILogger<UpdateQuestionCommandHandler> logger, IValidator<UpdateQuestionCommand> validator)
    {
        _unitOfWork = unitOfWork;
        _questionsRepository = questionsRepository;
        _categoriesRepository = categoriesRepository;
        _userService = userService;
        _currentUser = currentUser;
        _logger = logger;
        _validator = validator;
        
    }

    public async Task<QuestionResponse> Handle(UpdateQuestionCommand command, CancellationToken cancellationToken)
    {
        var commandValidationResult = _validator.Validate(command);
        
        if (!commandValidationResult.IsValid)
        {
            throw new ValidationException(EntityName.Question, commandValidationResult.ToDictionary());
        }

        var category = await _categoriesRepository.GetByIdAsync(command.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new IqpException(
                $"{EntityName.Question}.{EntityName.Category}", Errors.NotFound.ToString(), "Category not found",
                "The category with such id does not exist. Therefore question cannot be updated.");
        }

        var question = await _questionsRepository.GetByIdAsync(command.Id, cancellationToken);
        
        if (question is null)
        {
            throw new IqpException(
                EntityName.Question,Errors.NotFound.ToString(), "Not found", "The question with such id does not exist.");
        }
        
        var user = await _userService.GetUserByIdAsync(_currentUser.UserId.Value);

        question.Update(command.Title, command.Description, category, user!);
        _questionsRepository.Update(question);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Question with id {QuestionId} has been updated", question.Id);

        var isLiked = question.LikedBy.Any(u=>u.Id == _currentUser.UserId);
        
        return question.ToResponse(isLiked);
    }
}